using UnityEngine;

public class RShield : Tile
{
    public float radius;
    public float period;

    Tile player;
    float initTime;
    public void Init(Tile player) {
        this.player = player;
        initTime=Time.time;
    }
    void FixedUpdate()
    {
        transform.position=player.transform.position;
        UpdateRotation();
    }
    void UpdateRotation()
    {
        Vector2 center=player.transform.position;
        float t=Time.time-initTime;
        t=t/period*2*Mathf.PI;
        Vector2 offset=new Vector2(Mathf.Sin(t), Mathf.Cos(t));
        transform.localRotation=Quaternion.FromToRotation(Vector2.up, offset);
    }
    public override void takeDamage(Tile tileDamagingUs, int damageAmount, DamageType damageType)
    {
        base.takeDamage(tileDamagingUs, damageAmount, damageType);
        tileDamagingUs.takeDamage(tileDamagingUs, Mathf.Max(1, damageAmount/2), damageType);
    }
}