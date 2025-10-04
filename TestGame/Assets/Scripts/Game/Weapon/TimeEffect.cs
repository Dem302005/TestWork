using System.Collections;
using UnityEngine;

namespace Gamekit3D
{
    public class TimeEffect : MonoBehaviour
    {
        public Light staffLight;

        private Animation m_Animation;

        private void Awake()
        {
            m_Animation = GetComponent<Animation>();

            gameObject.SetActive(false);
        }

        public void Activate()
        {
            gameObject.SetActive(true);
            staffLight.enabled = true;

            if (m_Animation)
                m_Animation.Play();

            StartCoroutine(DisableAtEndOfAnimation());
        }

        private IEnumerator DisableAtEndOfAnimation()
        {
            yield return new WaitForSeconds(m_Animation.clip.length);

            gameObject.SetActive(false);
            staffLight.enabled = false;
        }
    }
}