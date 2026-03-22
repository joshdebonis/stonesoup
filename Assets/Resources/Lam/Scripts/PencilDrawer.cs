using System.Collections.Generic;
using UnityEngine;

namespace Lam
{
    public class PencilDrawer : MonoBehaviour
    {
        public float MinDistance = 0.1f;

        private LineRenderer m_line;
        private readonly List<Vector3> m_points = new List<Vector3>();
        private Camera m_cam;

        private void Awake()
        {
            m_line = GetComponent<LineRenderer>();
            m_cam = Camera.main;

            m_line.positionCount = 0;
            m_line.useWorldSpace = true;
        }

        private void Update()
        {
            
        }

        public void Draw(Vector3 worldPos)
        {
            worldPos.z = 0;

            if (m_points.Count == 0 || 
                Vector3.Distance(m_points[^1], worldPos) > MinDistance)
            {
                AddPoint(worldPos);
            }
        }

        void AddPoint(Vector3 point)
        {
            m_points.Add(point);
            m_line.positionCount = m_points.Count;
            m_line.SetPosition(m_points.Count - 1, point);
        }
    }
}
