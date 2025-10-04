using UnityEngine;

namespace Gamekit3D
{
    public class ParticleDeactivator : MonoBehaviour
    {
        public float duration;
        protected ParticleSystem m_ParticleSystem;

        protected float m_SinceActivation;

        private void Update()
        {
            m_SinceActivation += Time.deltaTime;
            if (m_SinceActivation > duration)
            {
                m_ParticleSystem.Stop(true);
                gameObject.SetActive(false);
            }
        }

        private void OnEnable()
        {
            m_ParticleSystem = GetComponent<ParticleSystem>();
            m_SinceActivation = 0;
        }
    }
}