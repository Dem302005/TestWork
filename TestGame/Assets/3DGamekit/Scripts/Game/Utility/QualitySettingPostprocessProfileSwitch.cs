using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
#if UNITY_EDITOR
using UnityEditor;
using System.Linq;
#endif

namespace Gamekit3D
{
    [ExecuteInEditMode]
    public class QualitySettingPostprocessProfileSwitch : MonoBehaviour
    {
        public QualitySettingEntry[] settings = new QualitySettingEntry[0];
        private PostProcessLayer.Antialiasing m_AntiAliasing;

        private PostProcessLayer m_Layer;
        private PostProcessProfile m_OriginalProfile;
        private int m_PickedSetting;

        private int m_PreviousQualitySetting;

        private PostProcessVolume m_Volume;

        private void Update()
        {
            var qualitySettingLevel = QualitySettings.GetQualityLevel();

            if (qualitySettingLevel != m_PreviousQualitySetting)
            {
                m_PreviousQualitySetting = qualitySettingLevel;
                DoSwitch();
            }
        }

        private void OnEnable()
        {
            m_PreviousQualitySetting = QualitySettings.GetQualityLevel();
            m_Volume = GetComponent<PostProcessVolume>();
            m_Layer = GetComponent<PostProcessLayer>();

            if (m_Volume == null)
            {
                enabled = false;
                return;
            }

            if (m_Layer != null)
                m_AntiAliasing = m_Layer.antialiasingMode;

            m_OriginalProfile = m_Volume.sharedProfile;

            DoSwitch();
        }

        private void OnDisable()
        {
            if (m_Volume != null)
                m_Volume.sharedProfile = m_OriginalProfile;
        }

        private void DoSwitch()
        {
            FindProperEntry();

            m_Volume.sharedProfile = m_PickedSetting == -1 || settings[m_PickedSetting].profile == null
                ? m_OriginalProfile
                : settings[m_PickedSetting].profile;

            if (m_Layer != null)
                m_Layer.antialiasingMode = m_PickedSetting == -1
                    ? m_AntiAliasing
                    : settings[m_PickedSetting].usedAntiAliasing;
        }

        private void FindProperEntry()
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

            m_PickedSetting = foundIdx;
        }

        [Serializable]
        public class QualitySettingEntry
        {
            public PostProcessProfile profile;
            public PostProcessLayer.Antialiasing usedAntiAliasing;
            public int minimumQualitySetting;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(QualitySettingPostprocessProfileSwitch))]
    public class QualitySettingPostprocessSwitchEditor : Editor
    {
        private int m_OpenedSettings;
        private QualitySettingPostprocessProfileSwitch m_Target;

        private int[] remainingQuality;
        private string[] remainingQualityNames;

        private void OnEnable()
        {
            m_Target = target as QualitySettingPostprocessProfileSwitch;
            m_OpenedSettings = -1;

            if (m_Target.settings != null)
                for (var i = 0; i < m_Target.settings.Length; ++i)
                    if (m_Target.settings[i].minimumQualitySetting >= QualitySettings.names.Length)
                    {
                        ArrayUtility.RemoveAt(ref m_Target.settings, i);
                        i--;
                    }

            GetRemainingQualitySetting();
        }

        public override void OnInspectorGUI()
        {
            if (remainingQuality.Length > 0)
            {
                var selected = EditorGUILayout.Popup("Add Quality Settings", -1, remainingQualityNames);
                if (selected != -1)
                {
                    Undo.RecordObject(target, "Added new Quality Setting in LayerCUllDistance");

                    var newEntry =
                        new QualitySettingPostprocessProfileSwitch.QualitySettingEntry();

                    newEntry.minimumQualitySetting = remainingQuality[selected];

                    ArrayUtility.Add(ref m_Target.settings, newEntry);

                    EditorUtility.SetDirty(m_Target);

                    ArrayUtility.RemoveAt(ref remainingQualityNames, selected);
                    ArrayUtility.RemoveAt(ref remainingQuality, selected);
                }
            }

            for (var i = 0; i < m_Target.settings.Length; ++i)
            {
                var opened = EditorGUILayout.Foldout(m_OpenedSettings == i,
                    "Quality : " + QualitySettings.names[m_Target.settings[i].minimumQualitySetting]);

                if (opened)
                {
                    m_OpenedSettings = i;
                    DrawSetting(i);
                }
                else if (m_OpenedSettings == i)
                {
                    m_OpenedSettings = -1;
                }
            }
        }

        private void GetRemainingQualitySetting()
        {
            remainingQuality = new int[QualitySettings.names.Length];
            for (var i = 0; i < remainingQuality.Length; ++i)
                remainingQuality[i] = i;

            for (var i = 0; i < m_Target.settings.Length; ++i)
                if (remainingQuality.Contains(m_Target.settings[i].minimumQualitySetting))
                    ArrayUtility.Remove(ref remainingQuality, m_Target.settings[i].minimumQualitySetting);

            remainingQualityNames = new string[remainingQuality.Length];
            for (var i = 0; i < remainingQuality.Length; ++i)
                remainingQualityNames[i] = QualitySettings.names[remainingQuality[i]];
        }

        private void DrawSetting(int index)
        {
            var setting = m_Target.settings[index];

            GUILayout.FlexibleSpace();
            if (m_Target.settings.Length > 1 && GUILayout.Button("Remove", GUILayout.Width(64)))
            {
                Undo.RecordObject(m_Target,
                    "Removed quality setting " + QualitySettings.names[m_Target.settings[index].minimumQualitySetting]);
                ArrayUtility.RemoveAt(ref m_Target.settings, index);
                EditorUtility.SetDirty(m_Target);
                GetRemainingQualitySetting();
                m_OpenedSettings = -1;
            }
            else
            {
                var newProfile =
                    EditorGUILayout.ObjectField("Profile", setting.profile, typeof(PostProcessProfile), false) as
                        PostProcessProfile;

                if (newProfile != setting.profile)
                {
                    Undo.RecordObject(m_Target,
                        "Changed profile for setting " +
                        QualitySettings.names[m_Target.settings[index].minimumQualitySetting]);
                    setting.profile = newProfile;
                    EditorUtility.SetDirty(m_Target);
                }

                var antiAliasing =
                    (PostProcessLayer.Antialiasing)EditorGUILayout.EnumPopup("Antialiasing", setting.usedAntiAliasing);
                if (antiAliasing != setting.usedAntiAliasing)
                {
                    Undo.RecordObject(m_Target,
                        "Changed antialiasing method for setting " +
                        QualitySettings.names[m_Target.settings[index].minimumQualitySetting]);
                    setting.usedAntiAliasing = antiAliasing;
                    EditorUtility.SetDirty(m_Target);
                }
            }
        }
    }
#endif
}