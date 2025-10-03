using System.Collections;
using UnityEngine;

namespace Gamekit3D
{
    public class HealthUI : MonoBehaviour
    {
        protected const float k_HeartIconAnchorWidth = 0.041f;
        public Damageable representedDamageable;
        public GameObject healthIconPrefab;

        protected readonly int m_HashActivePara = Animator.StringToHash("Active");
        protected readonly int m_HashInactiveState = Animator.StringToHash("Inactive");

        protected Animator[] m_HealthIconAnimators;

        private IEnumerator Start()
        {
            if (representedDamageable == null)
                yield break;

            yield return null;

            m_HealthIconAnimators = new Animator[representedDamageable.maxHitPoints];

            for (var i = 0; i < representedDamageable.maxHitPoints; i++)
            {
                var healthIcon = Instantiate(healthIconPrefab);
                healthIcon.transform.SetParent(transform);
                var healthIconRect = healthIcon.transform as RectTransform;
                healthIconRect.anchoredPosition = Vector2.zero;
                healthIconRect.sizeDelta = Vector2.zero;
                healthIconRect.anchorMin += new Vector2(k_HeartIconAnchorWidth, 0f) * i;
                healthIconRect.anchorMax += new Vector2(k_HeartIconAnchorWidth, 0f) * i;
                m_HealthIconAnimators[i] = healthIcon.GetComponent<Animator>();

                if (representedDamageable.currentHitPoints < i + 1)
                {
                    m_HealthIconAnimators[i].Play(m_HashInactiveState);
                    m_HealthIconAnimators[i].SetBool(m_HashActivePara, false);
                }
            }
        }

        public void ChangeHitPointUI(Damageable damageable)
        {
            if (m_HealthIconAnimators == null)
                return;

            for (var i = 0; i < m_HealthIconAnimators.Length; i++)
                m_HealthIconAnimators[i].SetBool(m_HashActivePara, damageable.currentHitPoints >= i + 1);
        }
    }
}