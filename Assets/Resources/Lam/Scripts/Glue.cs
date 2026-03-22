using UnityEngine;

namespace Lam
{
    public class Glue : Tile
    {
        public GameObject GlueBlobPrefab;
        public override void useAsItem(Tile tileUsingUs)
        {
            base.useAsItem(tileUsingUs);
            
            Debug.Log("Use Glue");
            Instantiate(GlueBlobPrefab, transform.position, Quaternion.identity);
            takeDamage(this, 1);
        }
    }
}
