using UnityEngine;

namespace Gamekit3D.GameCommands
{
    [RequireComponent(typeof(Collider))]
    public class Collectable : MonoBehaviour
    {
        public new Collider collider;
        public LayerMask layers;
        public GameObject collectEffect;
        public AudioClip onCollectAudio;
        public bool disableOnCollect;

        private void Reset()
        {
            collider = GetComponent<Collider>();
            collider.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (CanCollect(other))
                Collect(other);
        }

        protected virtual void Collect(Collider other)
        {
            if (collectEffect) collectEffect.SetActive(true);
            if (onCollectAudio)
            {
                var audio = GetComponent<AudioSource>();
                if (audio) audio.PlayOneShot(onCollectAudio);
            }

            var collector = other.GetComponent<Collector>();
            if (collector)
                collector.OnCollect(this);
            if (disableOnCollect)
                gameObject.SetActive(false);
        }

        private bool CanCollect(Collider other)
        {
            return 0 != (layers.value & (1 << other.gameObject.layer));
        }
    }
}