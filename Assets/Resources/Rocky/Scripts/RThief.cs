using UnityEngine;

public class RThief : BasicAICreature
{
    Tile runningFromTile;
    float timeSinceStolen;
    public override void FixedUpdate()
    {
        if(tileWereHolding==null)
            base.FixedUpdate();
        else
        {
            Vector2 targetGlobalPos = Tile.toWorldCoord(_targetGridPos.x, _targetGridPos.y);
            if (Vector2.Distance(transform.position, targetGlobalPos) <= 15f) {
                Vector2 toTargetPos = -(targetGlobalPos - (Vector2)transform.position).normalized;
                moveViaVelocity(toTargetPos, moveSpeed, moveAcceleration);
                // Figure out which direction we're going to face. 
                // Prioritize side and down.
                if (_anim != null) {
                    if (toTargetPos.x >= 0) {
                        _sprite.flipX = false;
                    }
                    else {
                        _sprite.flipX = true;
                    }
                    // Make sure we're marked as walking.
                    _anim.SetBool("Walking", true);
                    if (Mathf.Abs(toTargetPos.x) > 0 && Mathf.Abs(toTargetPos.x) > Mathf.Abs(toTargetPos.y)) {
                        _anim.SetInteger("Direction", 1);
                    }
                    else if (toTargetPos.y > 0 && toTargetPos.y > Mathf.Abs(toTargetPos.x)) {
                        _anim.SetInteger("Direction", 0);
                    }
                    else if (toTargetPos.y < 0 && Mathf.Abs(toTargetPos.y) > Mathf.Abs(toTargetPos.x)) {
                        _anim.SetInteger("Direction", 2);
                    }
                }
            }
            else {
                moveViaVelocity(Vector2.zero, 0, moveAcceleration);
                if (_anim != null) {
                    _anim.SetBool("Walking", false);
                }
            }
        }
        if (timeSinceStolen > 3 && tileWereHolding!=null) {
            tileWereHolding.useAsItem(this);
            timeSinceStolen=Time.time+Random.Range(-1f,1f);
        }
    }
    public override void tileDetected(Tile detectedTile)
    {
        base.tileDetected(detectedTile);
        if(detectedTile.tileWereHolding!=null)
            _targetGridPos = Tile.toGridCoord(detectedTile.transform.position.x, detectedTile.transform.position.y);
    }
    public override void tileNoLongerDetected(Tile detectedTile)
    {
        base.tileNoLongerDetected(detectedTile);
        _targetGridPos = Tile.toGridCoord(globalX, globalY);
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if(tileWereHolding!=null) return;
        Tile tile = collision.gameObject.GetComponent<Tile>();
        if (tile != null) {
            if (tile.tileWereHolding != null) {
                Tile stolenTile = tile.tileWereHolding;
                tile.tileWereHolding.dropped(tile);
                if (tile.tileWereHolding == null) {
                    stolenTile.pickUp(this);
                    runningFromTile=tile;
                    timeSinceStolen=Time.time+Random.Range(-1f,1f);
                }
            }
        }
    }
}