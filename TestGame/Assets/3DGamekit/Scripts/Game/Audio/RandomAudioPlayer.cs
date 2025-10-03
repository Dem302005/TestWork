using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


namespace Gamekit3D
{
    [RequireComponent(typeof(AudioSource))]
    public class RandomAudioPlayer : MonoBehaviour
    {
        public bool randomizePitch = true;
        public float pitchRandomRange = 0.2f;
        public float playDelay;
        public SoundBank defaultBank = new SoundBank();
        public MaterialAudioOverride[] overrides;

        [HideInInspector] public bool playing;

        [HideInInspector] public bool canPlay;

        protected AudioSource m_Audiosource;
        protected Dictionary<Material, SoundBank[]> m_Lookup = new Dictionary<Material, SoundBank[]>();

        public AudioSource audioSource => m_Audiosource;

        public AudioClip clip { get; private set; }

        private void Awake()
        {
            m_Audiosource = GetComponent<AudioSource>();
            for (var i = 0; i < overrides.Length; i++)
                foreach (var material in overrides[i].materials)
                    m_Lookup[material] = overrides[i].banks;
        }

        /// <summary>
        ///     Will pick a random clip to play in the assigned list. If you pass a material, it will try to find an
        ///     override for that materials or play the default clip if none can ben found.
        /// </summary>
        /// <param name="overrideMaterial"></param>
        /// <returns> Return the choosen audio clip, null if none </returns>
        public AudioClip PlayRandomClip(Material overrideMaterial, int bankId = 0)
        {
#if UNITY_EDITOR
            //UnityEditor.EditorGUIUtility.PingObject(overrideMaterial);
#endif
            if (overrideMaterial == null) return null;
            return InternalPlayRandomClip(overrideMaterial, bankId);
        }

        /// <summary>
        ///     Will pick a random clip to play in the assigned list.
        /// </summary>
        public void PlayRandomClip()
        {
            clip = InternalPlayRandomClip(null, 0);
        }

        private AudioClip InternalPlayRandomClip(Material overrideMaterial, int bankId)
        {
            SoundBank[] banks = null;
            var bank = defaultBank;
            if (overrideMaterial != null)
                if (m_Lookup.TryGetValue(overrideMaterial, out banks))
                    if (bankId < banks.Length)
                        bank = banks[bankId];
            if (bank.clips == null || bank.clips.Length == 0)
                return null;
            var clip = bank.clips[Random.Range(0, bank.clips.Length)];

            if (clip == null)
                return null;

            m_Audiosource.pitch =
                randomizePitch ? Random.Range(1.0f - pitchRandomRange, 1.0f + pitchRandomRange) : 1.0f;
            m_Audiosource.clip = clip;
            m_Audiosource.PlayDelayed(playDelay);

            return clip;
        }

        [Serializable]
        public class MaterialAudioOverride
        {
            public Material[] materials;
            public SoundBank[] banks;
        }

        [Serializable]
        public class SoundBank
        {
            public string name;
            public AudioClip[] clips;
        }
    }
}