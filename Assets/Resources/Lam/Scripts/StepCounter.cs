using System;
using TMPro;
using UnityEditor.Shaders;
using UnityEngine;

namespace Lam
{
    public class StepCounter : Tile
    {
        private bool m_isCounting = false;
        public float MinDistanceToCountAsStep = 0.5f;
        public TMP_Text Text;
        
        private Vector3 m_lastPosition;
        private int m_currentStep = 0;
        
        public override void pickUp(Tile tilePickingUsUp)
        {
            base.pickUp(tilePickingUsUp);
            m_isCounting = true;
        }

        public override void dropped(Tile tileDroppingUs)
        {
            base.dropped(tileDroppingUs);
            m_isCounting = false;
        }

        private void Update()
        {
            if (m_isCounting)
            {
                if (Vector3.Distance(transform.position, m_lastPosition) > MinDistanceToCountAsStep)
                {
                    m_lastPosition = transform.position;
                    m_currentStep++;
                    string result = m_currentStep.ToString("D4");
                    Text.text = result;
                }
            }
        }
    }
}
