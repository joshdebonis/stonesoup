using UnityEngine;

public class RShieldTile : Tile
{
    public RShield shieldPrefab;

    RShield instShield;
	public override void dropped(Tile tileDroppingUs) {
        base.dropped(tileDroppingUs);
        Destroy(instShield.gameObject);
        GetComponentInChildren<SpriteRenderer>().enabled=true;
    }
	public override void pickUp(Tile tilePickingUsUp) {
        base.pickUp(tilePickingUsUp);
        RShield inst=Instantiate(shieldPrefab).GetComponent<RShield>();
        inst.Init(tilePickingUsUp);
        GetComponentInChildren<SpriteRenderer>().enabled=false;
        instShield=inst;
    }
}