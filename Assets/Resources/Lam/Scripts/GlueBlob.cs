using System;
using System.Collections.Generic;
using UnityEngine;

namespace Lam
{
    public class GlueBlob : MonoBehaviour
    {
        private Dictionary<Tile, RigidbodyType2D> m_originalBodyTypes = new Dictionary<Tile, RigidbodyType2D>();

        private void Awake()
        {
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out Tile tile))
            {
                if (m_originalBodyTypes.ContainsKey(tile))
                {
                    return;
                }

                if (tile.body != null && tile is not Player)
                {
                    m_originalBodyTypes.Add(tile, tile.body.bodyType);
                    tile.body.bodyType = RigidbodyType2D.Static;
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.TryGetComponent(out Tile tile))
            {
                if (m_originalBodyTypes.ContainsKey(tile))
                {
                    tile.body.bodyType = m_originalBodyTypes[tile];
                    m_originalBodyTypes.Remove(tile);
                }
            }
        }
    }
}