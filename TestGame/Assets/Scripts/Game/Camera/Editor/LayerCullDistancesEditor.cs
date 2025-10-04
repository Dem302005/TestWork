using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Gamekit3D.Cameras
{
    [CustomEditor(typeof(LayerCullDistances))]
    public class LayerCullDistancesEditor : Editor
    {
        private int m_OpenedSettings;
        private LayerCullDistances m_Target;

        private int[] remainingQuality;
        private string[] remainingQualityNames;

        private void OnEnable()
        {
            m_Target = target as LayerCullDistances;
            m_OpenedSettings = -1;

            if (m_Target.settings == null)
                m_Target.Reset();
            else
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
                    m_Target.AddNewSetting(remainingQuality[selected]);
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
                m_OpenedSettings = -1;
                GetRemainingQualitySetting();
            }
            else
            {
                EditorGUI.BeginChangeCheck();
                var nearPlane = EditorGUILayout.FloatField("Near Plane", setting.nearPlane);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(target, "Changed near plane");
                    setting.nearPlane = nearPlane;
                    InternalEditorUtility.RepaintAllViews();
                }

                EditorGUI.BeginChangeCheck();
                var farPlane = EditorGUILayout.FloatField("Far Plane", setting.farPlane);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(target, "Changed far plane");
                    setting.farPlane = farPlane;
                    InternalEditorUtility.RepaintAllViews();
                }

                for (var i = 0; i < setting.distances.Length; i++)
                {
                    var name = LayerMask.LayerToName(i);
                    if (name != "")
                    {
                        EditorGUI.BeginChangeCheck();
                        var newValue = EditorGUILayout.Slider(name + " (" + i + ")", setting.distances[i],
                            setting.nearPlane,
                            setting.farPlane);
                        if (EditorGUI.EndChangeCheck())
                        {
                            Undo.RecordObject(target, "Changed culling distance for " + name + " layer");
                            setting.distances[i] = newValue;
                            InternalEditorUtility.RepaintAllViews();
                        }
                    }
                }
            }
        }
    }
}