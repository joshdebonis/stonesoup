using UnityEngine;

public class drug : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        Tile player = other.GetComponent<Tile>();
        if (player == null) return;
        if (!player.hasTag(TileTags.Player)) return;

        foreach (var pig in FindObjectsOfType<pigInfection>())
        {
            pig.CureIfTargetIs(player);
        }

        Destroy(gameObject);
    }
    
}