using System;
using UnityEngine;

namespace Lam
{
    public class Pencil : Tile
    {
        public GameObject PencilDrawerPrefab;
        private bool m_isDrawing = false;
        
        private PencilDrawer m_pencilDrawer;
        public override void useAsItem(Tile tileUsingUs)
        {
             m_isDrawing = !m_isDrawing;
             if (m_isDrawing)
             {
                 m_pencilDrawer = Instantiate(PencilDrawerPrefab, transform.position, Quaternion.identity).GetComponent<PencilDrawer>();
             }
            
        }

        private void Update()
        {
            if (m_isDrawing)
            {
                m_pencilDrawer.Draw(transform.position);
            }
        }
    }
}
