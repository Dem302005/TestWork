using System.Collections;
using UnityEngine;

namespace Gamekit3D
{
    [RequireComponent(typeof(AudioSource))]
    public class AmbientAudio : MonoBehaviour
    {
        public float minPitch = 0.99f;
        public float maxPitch = 1.01f;
        public float minVolume = 0.5f;
        public float maxVolume = 0.9f;
        public bool randomDelays = true;

        public AudioListener audioListener;

        private new AudioSource audio;
        private int delayIndex;

        private WaitForSeconds[] delays;

        private WaitForSeconds Delay => delays[delayIndex++ % delays.Length];

        private IEnumerator Start()
        {
            audio = GetComponent<AudioSource>();
            if (audio.clip == null) yield break;
            if (audioListener == null) audioListener = FindObjectOfType<AudioListener>();
            audio.loop = false;
            delays = new WaitForSeconds[7];
            for (var i = 0; i < delays.Length; i++)
                delays[i] = new WaitForSeconds(audio.clip.length * Random.Range(1, 3));
            var loopDelay = new WaitForSeconds(audio.clip.length);
            while (true)
            {
                if (randomDelays)
                    yield return Delay;
                else
                    yield return loopDelay;
                if (audioListener != null && (audioListener.transform.position - transform.position).magnitude >
                    audio.maxDistance)
                    continue;
                audio.pitch = Random.Range(minPitch, maxPitch);
                audio.volume = Random.Range(minVolume, maxVolume);
                audio.Play();
            }
        }
    }
}