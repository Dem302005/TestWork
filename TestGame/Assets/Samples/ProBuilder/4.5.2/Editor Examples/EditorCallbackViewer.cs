using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.ProBuilder;
using UnityEngine;
using UnityEngine.ProBuilder;
using EditorUtility = UnityEditor.ProBuilder.EditorUtility;
using Math = System.Math;

namespace ProBuilder.EditorExamples
{
    internal sealed class EditorCallbackViewer : EditorWindow
    {
        private bool m_Collapse = true;
        private readonly List<string> m_Logs = new List<string>();
        private Vector2 m_Scroll = Vector2.zero;

        private static Color logBackgroundColor => EditorGUIUtility.isProSkin
            ? new Color(.15f, .15f, .15f, .5f)
            : new Color(.8f, .8f, .8f, 1f);

        private static Color disabledColor =>
            EditorGUIUtility.isProSkin ? new Color(.3f, .3f, .3f, .5f) : new Color(.8f, .8f, .8f, 1f);

        private void OnEnable()
        {
            ProBuilderEditor.selectModeChanged += SelectModeChanged;
            EditorUtility.meshCreated += MeshCreated;
            ProBuilderEditor.selectionUpdated += SelectionUpdated;
            ProBuilderEditor.beforeMeshModification += BeforeMeshModification;
            ProBuilderEditor.afterMeshModification += AfterMeshModification;
            EditorMeshUtility.meshOptimized += MeshOptimized;
        }

        private void OnDisable()
        {
            ProBuilderEditor.selectModeChanged -= SelectModeChanged;
            EditorUtility.meshCreated -= MeshCreated;
            ProBuilderEditor.selectionUpdated -= SelectionUpdated;
            ProBuilderEditor.beforeMeshModification -= BeforeMeshModification;
            ProBuilderEditor.afterMeshModification -= AfterMeshModification;
            EditorMeshUtility.meshOptimized -= MeshOptimized;
        }

        private void OnGUI()
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar);

            GUILayout.FlexibleSpace();

            GUI.backgroundColor = m_Collapse ? disabledColor : Color.white;
            if (GUILayout.Button("Collapse", EditorStyles.toolbarButton))
                m_Collapse = !m_Collapse;
            GUI.backgroundColor = Color.white;

            if (GUILayout.Button("Clear", EditorStyles.toolbarButton))
                m_Logs.Clear();

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Callback Log", EditorStyles.boldLabel);
            GUILayout.EndHorizontal();

            var r = GUILayoutUtility.GetLastRect();
            r.x = 0;
            r.y = r.y + r.height + 6;
            r.width = position.width;
            r.height = position.height;

            GUILayout.Space(4);

            m_Scroll = GUILayout.BeginScrollView(m_Scroll);

            var len = m_Logs.Count;
            var min = Math.Max(0, len - 1024);

            for (var i = len - 1; i >= min; i--)
            {
                if (m_Collapse &&
                    i > 0 &&
                    i < len - 1 &&
                    m_Logs[i].Equals(m_Logs[i - 1]) &&
                    m_Logs[i].Equals(m_Logs[i + 1]))
                    continue;

                GUILayout.Label(string.Format("{0,3}: {1}", i, m_Logs[i]));
            }

            GUILayout.EndScrollView();
        }

        [MenuItem("Tools/ProBuilder/API Examples/Log Callbacks Window")]
        private static void MenuInitEditorCallbackViewer()
        {
            GetWindow<EditorCallbackViewer>(true, "ProBuilder Callbacks", true).Show();
        }

        private void BeforeMeshModification(IEnumerable<ProBuilderMesh> selection)
        {
            AddLog("Began Moving Vertices");
        }

        private void AfterMeshModification(IEnumerable<ProBuilderMesh> selection)
        {
            AddLog("Finished Moving Vertices");
        }

        private void SelectModeChanged(SelectMode mode)
        {
            AddLog("Selection Mode Changed: " + mode);
        }

        private void MeshCreated(ProBuilderMesh mesh)
        {
            AddLog("Instantiated new ProBuilder Object: " + mesh.name);
        }

        private void SelectionUpdated(IEnumerable<ProBuilderMesh> selection)
        {
            AddLog("Selection Updated: " +
                   string.Format("{0} objects selected.", selection != null ? selection.Count() : 0));
        }

        private void MeshOptimized(ProBuilderMesh pmesh, Mesh umesh)
        {
            AddLog(string.Format("Mesh {0} rebuilt", pmesh.name));
        }

        private void AddLog(string summary)
        {
            m_Logs.Add(summary);
            Repaint();
        }
    }
}