using System.Collections;
using System.Security.Cryptography;
using UnityEngine;

public class RHealthTransferer : Tile
{
	[SerializeField] 
	[EnumFlagsAttribute]
	public TileTags tagsToDetect = 0;

	public float detectionRadius = 12;
    public Vector2 detectFrom, detectTo;
	// We randomize how much time we wait until the next poll, mainly to ensure that polling for a lot of the 
	// tile detectors is spread out across multiple frames (i.e. everyone trying to poll on the same frame would
    // probably introduce lag on that frame)
	public float minTimeBetweenPolls = 0.5f;
	public float maxTimeBetweenPolls = 1f;
    [Header("Animation")]
    public SpriteRenderer spr;
    public Sprite spr1, spr2;

	protected float _timeTillNextPoll;

	protected Tile _parentTile;

	public LayerMask layerMask = 0x1 + 0x200;

    void OnDrawGizmosSelected() {
        Gizmos.color=Color.red;
        Gizmos.DrawWireSphere((Vector3)detectFrom+transform.position, detectionRadius);
        Gizmos.color=Color.blue;
        Gizmos.DrawWireSphere((Vector3)detectTo+transform.position, detectionRadius);
    }

	void Start() {
		_timeTillNextPoll = Random.Range(minTimeBetweenPolls, maxTimeBetweenPolls);
		gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
	}

	void Update() {
		_timeTillNextPoll -= Time.deltaTime;
		if (_timeTillNextPoll <= 0) {
            Tile fromTile=GetTileAt((Vector3)detectFrom+transform.position);
            Tile toTile=GetTileAt((Vector3)detectTo+transform.position);
            TransferHealth(fromTile, toTile);
			_timeTillNextPoll = Random.Range(minTimeBetweenPolls, maxTimeBetweenPolls);
		}
	}

    void TransferHealth(Tile from, Tile to) {
        if(from==null || to==null) return;
        Debug.Log("transfer health");
        to.health+=1;
        from.takeDamage(this, 1);
        StartCoroutine(Anim());
    }

    IEnumerator Anim() {
        spr.sprite=spr2;
        yield return new WaitForSeconds(minTimeBetweenPolls*.7f);
        spr.sprite=spr1;
    }

	// Can be called by something else to force a poll at key moments (i.e. if an enemy always wants to do a poll before taking a step)
    Tile GetTileAt(Vector2 point) {
        Collider2D cd = Physics2D.OverlapCircle(point, detectionRadius, layerMask);
			
        Tile otherTile = cd?cd.GetComponent<Tile>():null;
        return otherTile;
    }
}