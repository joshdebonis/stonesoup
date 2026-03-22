using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pigInfection : BasicAICreature {

	// How much force we inflict if something collides with us.
	public float damageForce = 1000;
	public int damageAmount = 1;


	// If we're chasing a friendly object, it'll be stored here.
	protected Tile _tileWereChasing = null;
	public Tile tileWereChasing {
		get { return _tileWereChasing; }
		set { _tileWereChasing = value; }
	}

	// How far the object we're chasing has to be before we stop chasing it. 
	public float maxDistanceToContinueChase = 12f;

	// When chasing we either
	// a. Choose our next target position once we reach our current one
	// or
	// b. Choose our next target position if we've gone too long without reaching our target position.
	// or
	// c. Recalculate our target position when we collide with something (not the target tile).
	protected float _timeSinceLastStep = 0f;

	// -------- 新增：颜色锁定 --------
	SpriteRenderer _sr;
	Color _pigColor = Color.white;
	
	public enum PigInfectionType { None, Red, Green, Purple }
	[SerializeField] PigInfectionType _type = PigInfectionType.Green;
	
	bool infected = false;
	Tile infectedPlayer;
	float infectionTimer = 0f;
	public float tickSeconds = 10f;
	
	Color _playerPrevColor = Color.white;

	void Awake()
	{
		_sr = GetComponent<SpriteRenderer>();
		if (_sr != null)
		{
			_pigColor = _sr.color;
			_type = GuessTypeFromColor(_pigColor);
			
		}
		ApplyPigVisual();
	}
	void OnEnable()
	{
		ApplyPigVisual();
	}

	void LateUpdate()
	{
		// 防止别的脚本把猪颜色刷掉
		ApplyPigVisual();
	}

	void ApplyPigVisual()
	{
		if (_sr == null) _sr = GetComponent<SpriteRenderer>();
		if (_sr != null && _sr.color != _pigColor)
			_sr.color = _pigColor;
	}

	// 外部可调用：把猪变色 + 改感染类型（红/绿/紫）
	public void SetPigType(PigInfectionType t)
	{
		_type = t;
		_pigColor = PigTypeToColor(t);
		ApplyPigVisual();
	}
	// -------- 新增结束 --------
	public virtual void Update() {
		_timeSinceLastStep += Time.deltaTime;
		Vector2 targetGlobalPos = Tile.toWorldCoord(_targetGridPos.x, _targetGridPos.y);
		float distanceToTarget = Vector2.Distance(transform.position, targetGlobalPos);
		if (distanceToTarget <= GRID_SNAP_THRESHOLD || _timeSinceLastStep >= 2f) {
			takeStep();
		}
		updateSpriteSorting();
		
		// 感染掉血逻辑
		if (infected && infectedPlayer != null)
		{
			infectionTimer += Time.deltaTime;

			if (infectionTimer >= tickSeconds)
			{
				infectedPlayer.takeDamage(this, damageAmount);
				infectionTimer = 0f;
			}
		}
	}

	protected override void takeStep() {

		_takingCorrectingStep = false;
		_timeSinceLastStep = 0f;

		if (_tileWereChasing == null) {
			_targetGridPos = toGridCoord(globalX, globalY);
			return;
		}

		// First, figure out if the target is too far away
		float distanceToTile = Vector2.Distance(transform.position, _tileWereChasing.transform.position);
		if (distanceToTile > maxDistanceToContinueChase) {
			_tileWereChasing = null;
			return;
		}

		// We do this to re-calculate exactly where we are right now. 
		_targetGridPos = Tile.toGridCoord(globalX, globalY);

		_neighborPositions.Clear();

		// Otherwise, we're going to look at all potential neighbors and then figure out the best one to go to.
		Vector2 upGridNeighbor = new Vector2(_targetGridPos.x, _targetGridPos.y+1);
		if (pathIsClear(toWorldCoord(upGridNeighbor), CanOverlapIgnoreTargetTile)) {
			_neighborPositions.Add(upGridNeighbor);
		}
		Vector2 upRightGridNeighbor = new Vector2(_targetGridPos.x+1, _targetGridPos.y+1);
		if (pathIsClear(toWorldCoord(upRightGridNeighbor), CanOverlapIgnoreTargetTile)) {
			_neighborPositions.Add(upRightGridNeighbor);
		}
		Vector2 rightGridNeighbor = new Vector2(_targetGridPos.x+1, _targetGridPos.y);
		if (pathIsClear(toWorldCoord(rightGridNeighbor), CanOverlapIgnoreTargetTile)) {
			_neighborPositions.Add(rightGridNeighbor);
		}
		Vector2 downRightGridNeighbor= new Vector2(_targetGridPos.x+1, _targetGridPos.y-1);
		if (pathIsClear(toWorldCoord(downRightGridNeighbor), CanOverlapIgnoreTargetTile)) {
			_neighborPositions.Add(downRightGridNeighbor);
		}
		Vector2 downGridNeighbor = new Vector2(_targetGridPos.x, _targetGridPos.y-1);
		if (pathIsClear(toWorldCoord(downGridNeighbor), CanOverlapIgnoreTargetTile)) {
			_neighborPositions.Add(downGridNeighbor);
		}
		Vector2 downLeftGridNeighbor= new Vector2(_targetGridPos.x-1, _targetGridPos.y-1);
		if (pathIsClear(toWorldCoord(downLeftGridNeighbor), CanOverlapIgnoreTargetTile)) {
			_neighborPositions.Add(downLeftGridNeighbor);
		}
		Vector2 leftGridNeighbor = new Vector2(_targetGridPos.x-1, _targetGridPos.y);
		if (pathIsClear(toWorldCoord(leftGridNeighbor), CanOverlapIgnoreTargetTile)) {
			_neighborPositions.Add(leftGridNeighbor);
		}
		Vector2 upLeftGridNeighbor= new Vector2(_targetGridPos.x-1, _targetGridPos.y+1);
		if (pathIsClear(toWorldCoord(upLeftGridNeighbor), CanOverlapIgnoreTargetTile)) {
			_neighborPositions.Add(upLeftGridNeighbor);
		}

		// Now, of the neighbor positions, pick the one that's closest. 
		float minDistance = distanceToTile;
		Vector2 minNeighbor = _targetGridPos;
		GlobalFuncs.shuffle(_neighborPositions);
		foreach (Vector2 neighborPos in _neighborPositions) {
			float distanceFromTarget = Vector2.Distance(Tile.toWorldCoord(neighborPos.x, neighborPos.y), _tileWereChasing.transform.position);
			if (distanceFromTarget < minDistance) {
				minNeighbor = neighborPos;
				minDistance = distanceFromTarget;
			}
		}
		if (minNeighbor == _targetGridPos) {
			// Couldn't get any closer, stop the chase!
			_tileWereChasing = null;
		}

		_targetGridPos = minNeighbor;

	}

	protected void takeCorrectionStep() {
		// We do this when we need to correct where we think we are
		// i.e. if we and another creature think we're both on the same gridpos, one of us needs to switch to a neighboring gridPos.
		_timeSinceLastStep = 0f;
		_takingCorrectingStep = true;
	
		_neighborPositions.Clear();

		// Otherwise, we're going to look at all potential neighbors and then figure out the best one to go to.
		Vector2 upGridNeighbor = new Vector2(_targetGridPos.x, _targetGridPos.y+1);
		if (pathIsClear(toWorldCoord(upGridNeighbor), dontOverlapWalls)) {
			_neighborPositions.Add(upGridNeighbor);
		}
		Vector2 upRightGridNeighbor = new Vector2(_targetGridPos.x+1, _targetGridPos.y+1);
		if (pathIsClear(toWorldCoord(upRightGridNeighbor), dontOverlapWalls)) {
			_neighborPositions.Add(upRightGridNeighbor);
		}
		Vector2 rightGridNeighbor = new Vector2(_targetGridPos.x+1, _targetGridPos.y);
		if (pathIsClear(toWorldCoord(rightGridNeighbor), dontOverlapWalls)) {
			_neighborPositions.Add(rightGridNeighbor);
		}
		Vector2 downRightGridNeighbor= new Vector2(_targetGridPos.x+1, _targetGridPos.y-1);
		if (pathIsClear(toWorldCoord(downRightGridNeighbor), dontOverlapWalls)) {
			_neighborPositions.Add(downRightGridNeighbor);
		}
		Vector2 downGridNeighbor = new Vector2(_targetGridPos.x, _targetGridPos.y-1);
		if (pathIsClear(toWorldCoord(downGridNeighbor), dontOverlapWalls)) {
			_neighborPositions.Add(downGridNeighbor);
		}
		Vector2 downLeftGridNeighbor= new Vector2(_targetGridPos.x-1, _targetGridPos.y-1);
		if (pathIsClear(toWorldCoord(downLeftGridNeighbor), dontOverlapWalls)) {
			_neighborPositions.Add(downLeftGridNeighbor);
		}
		Vector2 leftGridNeighbor = new Vector2(_targetGridPos.x-1, _targetGridPos.y);
		if (pathIsClear(toWorldCoord(leftGridNeighbor), dontOverlapWalls)) {
			_neighborPositions.Add(leftGridNeighbor);
		}
		Vector2 upLeftGridNeighbor= new Vector2(_targetGridPos.x-1, _targetGridPos.y+1);
		if (pathIsClear(toWorldCoord(upLeftGridNeighbor), dontOverlapWalls)) {
			_neighborPositions.Add(upLeftGridNeighbor);
		}

		if (_neighborPositions.Count > 0) {
			_targetGridPos = GlobalFuncs.randElem(_neighborPositions);
		}
		else {
			_targetGridPos += Vector2.up;
		}
	}


	// Colliding with a friendly should hurt it.
	
	void OnCollisionEnter2D(Collision2D collision)
	{
		// ① 猪碰猪：红+绿 => 紫（两只一起变紫）
		var otherPig = collision.gameObject.GetComponent<pigInfection>();
		if (otherPig != null && otherPig != this)
		{
			bool redGreen =
				(_type == PigInfectionType.Red && otherPig._type == PigInfectionType.Green) ||
				(_type == PigInfectionType.Green && otherPig._type == PigInfectionType.Red);

			if (redGreen)
			{
				SetPigType(PigInfectionType.Purple);
				otherPig.SetPigType(PigInfectionType.Purple);
			}

			// 猪碰猪不走玩家感染逻辑
			return;
		}

		// ② 猪碰玩家：给玩家叠加感染（红/绿 -> 紫）
		Tile otherTile = collision.gameObject.GetComponent<Tile>();
		if (otherTile == null) return;
		if (!otherTile.hasTag(tagsWeChase)) return;

		infected = true;
		infectedPlayer = otherTile;

		var tint = otherTile.GetComponent<HealthMeterTint>();
		if (tint != null) tint.enabled = false;

		var playerSR = otherTile.GetComponentInChildren<SpriteRenderer>();
		if (playerSR != null)
		{
			_playerPrevColor = playerSR.color;

			// 给玩家挂一个状态组件（没有就加）
			var status = otherTile.GetComponent<PlayerInfectionStatus>();
			if (status == null) status = otherTile.gameObject.AddComponent<PlayerInfectionStatus>();

			// 将猪的类型转成玩家感染
			status.Apply(PigTypeToPlayerInfection(_type));

			// 显示颜色：紫优先
			playerSR.color = PlayerInfectionStatus.ToColor(status.State);
		}
	}
	
		public void CureIfTargetIs(Tile player)
    {
        if (infected && infectedPlayer == player)
        {
            CureInfection();
        }
    }

    public void CureInfection()
    {
        infected = false;
        infectionTimer = 0f;

        if (infectedPlayer != null)
        {
            // 清玩家感染状态
            var status = infectedPlayer.GetComponent<PlayerInfectionStatus>();
            if (status != null) status.Clear();

            // 恢复玩家颜色（你想强制白色就用 white）
            var sr = infectedPlayer.GetComponentInChildren<SpriteRenderer>();
            if (sr != null) sr.color = Color.white;
        }

        infectedPlayer = null;
    }

    // ---- 追踪逻辑（原样保留）----
    public override void tileDetected(Tile otherTile)
    {
        if (_tileWereChasing == null && otherTile.hasTag(tagsWeChase))
        {
            _tileWereChasing = otherTile;
            takeStep();
        }
    }

    protected bool CanOverlapIgnoreTargetTile(RaycastHit2D hitResult)
    {
        Tile maybeResultTile = hitResult.transform.GetComponent<Tile>();
        if (maybeResultTile == _tileWereChasing) return true;
        return DefaultCanOverlapFunc(hitResult);
    }

    protected bool dontOverlapWalls(RaycastHit2D hitResult)
    {
        Tile maybeResultTile = hitResult.transform.GetComponent<Tile>();
        if (maybeResultTile != null && maybeResultTile.hasTag(TileTags.Wall)) return false;
        return true;
    }

    // ---- 工具函数 ----
    PigInfectionType GuessTypeFromColor(Color c)
    {
        // 你 prefab 上用纯红/纯绿最好；如果你用偏色，这里可以调 eps
        if (Approx(c, Color.red, 0.15f)) return PigInfectionType.Red;
        if (Approx(c, Color.green, 0.15f)) return PigInfectionType.Green;

        // 如果你想“不是红也不是绿就当 None”
        // return PigInfectionType.None;

        // 或者默认当 Green（看你）
        return PigInfectionType.Green;
    }

    bool Approx(Color a, Color b, float eps)
    {
        return Mathf.Abs(a.r - b.r) < eps &&
               Mathf.Abs(a.g - b.g) < eps &&
               Mathf.Abs(a.b - b.b) < eps;
    }

    Color PigTypeToColor(PigInfectionType t)
    {
        switch (t)
        {
            case PigInfectionType.Red: return Color.red;
            case PigInfectionType.Green: return Color.green;
            case PigInfectionType.Purple: return new Color(0.5f, 0f, 0.5f, 1f);
            default: return Color.white;
        }
    }

    PlayerInfectionStatus.Infection PigTypeToPlayerInfection(PigInfectionType t)
    {
        switch (t)
        {
            case PigInfectionType.Red: return PlayerInfectionStatus.Infection.Red;
            case PigInfectionType.Green: return PlayerInfectionStatus.Infection.Green;
            case PigInfectionType.Purple: return PlayerInfectionStatus.Infection.Purple;
            default: return PlayerInfectionStatus.Infection.None;
        }
    }
}

