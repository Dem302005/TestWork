using UnityEngine;

namespace Gamekit3D
{
    public class AudioPlayerOnEnable : MonoBehaviour
    {
        public RandomAudioPlayer player;
        public bool stopOnDisable;

        private void OnEnable()
        {
            player.PlayRandomClip();
        }

        private void OnDisable()
        {
            if (stopOnDisable)
                player.audioSource.Stop();
        }
    }
}