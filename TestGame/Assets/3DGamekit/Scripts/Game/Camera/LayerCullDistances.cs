using System;
using UnityEditor;
using UnityEngine;

namespace Gamekit3D.Cameras
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    public class LayerCullDistances : MonoBehaviour
    {
        public new Camera camera;

        public QualitySpecificSettings[] settings = new QualitySpecificSettings[0];
        private float[] computedDistances;

        private int pickedSetting;

        private void Start()
        {
            ComputeLayerCullDistances();
        }

        private void Update()
        {
            if (Application.isEditor)
                ComputeLayerCullDistances();
        }

        private void OnEnable()
        {
            camera = GetComponent<Camera>();
        }

        public void ComputeLayerCullDistances()
        {
            FindSettings();

            if (pickedSetting < 0 || pickedSetting >= settings.Length)
                return;

            camera.farClipPlane = settings[pickedSetting].farPlane;
            camera.nearClipPlane = settings[pickedSetting].nearPlane;
            camera.layerCullDistances = settings[pickedSetting].distances;
            camera.layerCullSpherical = true;

            computedDistances = new float[settings[pickedSetting].distances.Length];
            for (var i = 0; i < settings[pickedSetting].distances.Length; ++i)
                computedDistances[i] = settings[pickedSetting].distances[i] /
                                       (settings[pickedSetting].farPlane - settings[pickedSetting].nearPlane);

            Shader.SetGlobalFloatArray("_LayerCullDistances", computedDistances);
        }

        private void FindSettings()
        {
            var foundIdx = -1;
            var highestSetting = -1;
            var currentQualitySetting = QualitySettings.GetQualityLevel();

            for (var i = 0; i < settings.Length; ++i)
                if (settings[i].minimumQualitySetting <= currentQualitySetting &&
                    settings[i].minimumQualitySetting > highestSetting)
                {
                    highestSetting = settings[i].minimumQualitySetting;
                    foundIdx = i;
                }

            if (foundIdx == -1)
                //use the first one
                pickedSetting = 0;
            else
                pickedSetting = foundIdx;
        }

        [Serializable]
        public class QualitySpecificSettings
        {
            public int minimumQualitySetting;
            public float nearPlane = 0.3f;
            public float farPlane = 1000f;
            public float[] distances = new float[32];
        }

#if UNITY_EDITOR
        public void Reset()
        {
            settings = new QualitySpecificSettings[0];

            AddNewSetting(0);
        }

        public void AddNewSetting(int settingLevel)
        {
            var setting = new QualitySpecificSettings();

            setting.nearPlane = 0.3f;
            setting.farPlane = 5000.0f;
            setting.minimumQualitySetting = settingLevel;

            for (var i = 0; i < 32; ++i) setting.distances[i] = 1500;

            ArrayUtility.Add(ref settings, setting);
        }
#endif
    }
}