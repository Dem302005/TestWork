using UnityEngine;

namespace Gamekit3D
{
    [ExecuteInEditMode]
    public class GlobalShaderSettings : MonoBehaviour
    {
        [SerializeField] private float TopScale = 1;

        [SerializeField] private float NormalDetailScale = 1;

        [SerializeField] private float NoiseAmount = 1;

        [SerializeField] private float NoiseFalloff = 1;

        [SerializeField] private float NoiseScale = 1;

        [SerializeField] private float FresnelAmount = 0.5f;

        [SerializeField] private float FresnelPower = 0.5f;

        private void Update()
        {
            Shader.SetGlobalFloat("_TopScale", TopScale);
            Shader.SetGlobalFloat("_TopNormal2Scale", NormalDetailScale);
            Shader.SetGlobalFloat("_NoiseAmount", NoiseAmount);
            Shader.SetGlobalFloat("_NoiseFallOff", NoiseFalloff);
            Shader.SetGlobalFloat("_noiseScale", NoiseScale);
            Shader.SetGlobalFloat("_FresnelAmount", FresnelAmount);
            Shader.SetGlobalFloat("_FresnelPower", FresnelPower);
        }
    }
}