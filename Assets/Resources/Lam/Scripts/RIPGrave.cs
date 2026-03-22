using System;
using UnityEngine;

public class RIPGrave : Tile
{
    public Collider2D HeldCollider;
    public float PlayerMoveSpeedMultiplier = 0.5f;
    private float m_playerOriginalMoveSpeed;

    private void Start()
    {
        //HeldCollider.enabled = false;
    }

    public override void pickUp(Tile tilePickingUsUp)
    {
        base.pickUp(tilePickingUsUp);

        HeldCollider.enabled = true;
        
        if(tilePickingUsUp is Player)
        {
            Player player = tilePickingUsUp as Player;
            m_playerOriginalMoveSpeed = player.moveSpeed;
            player.moveSpeed *= PlayerMoveSpeedMultiplier;
        }
    }

    public override void dropped(Tile tileDroppingUs)
    {
        base.dropped(tileDroppingUs);
        
        HeldCollider.enabled = false;
        
        if (tileDroppingUs is Player)
        {
            Player player = tileDroppingUs as Player;
            player.moveSpeed = m_playerOriginalMoveSpeed;
        }
    }
}
