using UnityEngine;

namespace Gamekit3D
{
    public class SoundTrackVolume : MonoBehaviour
    {
        public LayerMask layers;
        private SoundTrack soundTrack;

        private void OnEnable()
        {
            soundTrack = GetComponentInParent<SoundTrack>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (0 != (layers.value & (1 << other.gameObject.layer)))
                soundTrack.PushTrack(name);
        }

        private void OnTriggerExit(Collider other)
        {
            if (0 != (layers.value & (1 << other.gameObject.layer)))
                soundTrack.PopTrack();
        }
    }
}