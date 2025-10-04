using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace Gamekit3D
{
    [RequireComponent(typeof(Slider))]
    public class MixerSliderLink : MonoBehaviour
    {
        public AudioMixer mixer;
        public string mixerParameter;

        public float maxAttenuation;
        public float minAttenuation = -80.0f;

        protected Slider m_Slider;


        private void Awake()
        {
            m_Slider = GetComponent<Slider>();

            float value;
            mixer.GetFloat(mixerParameter, out value);

            m_Slider.value = (value - minAttenuation) / (maxAttenuation - minAttenuation);

            m_Slider.onValueChanged.AddListener(SliderValueChange);
        }


        private void SliderValueChange(float value)
        {
            mixer.SetFloat(mixerParameter, minAttenuation + value * (maxAttenuation - minAttenuation));
        }
    }
}