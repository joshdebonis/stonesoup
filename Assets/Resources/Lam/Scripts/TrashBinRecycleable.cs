using UnityEngine;

namespace Lam
{
    public class TrashBinRecycleable : Tile
    {
        public TileTags IgnoreTags;
        private void OnTriggerEnter2D(Collider2D other)
        {
            Tile tile = other.GetComponent<Tile>();
            
            if(tile == null)
            {
                return;
            }
            
            if (tile.tags.HasFlag(IgnoreTags))
            {
                return;
            }

            if (tile == null)
                return;

            Renderer rend = other.GetComponent<Renderer>();
            if (rend != null)
            {
                rend.enabled = false;
            }

            tile.enabled = false;
            
            tile.gameObject.SetActive(false);

            Debug.Log($"Tile {other.name} recycled.");
        }
    }
}