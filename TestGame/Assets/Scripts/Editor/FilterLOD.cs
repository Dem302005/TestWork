using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FilterLOD : EditorWindow
{
    private readonly List<GameObject> lodGroupWithUnaprented = new List<GameObject>();
    private Vector2 scrollPos;

    private void OnEnable()
    {
        scrollPos = Vector2.zero;
        lodGroupWithUnaprented.Clear();
        var gos = SceneManager.GetActiveScene().GetRootGameObjects();

        for (var i = 0; i < gos.Length; ++i) HierarchicalDown(gos[i]);
    }

    private void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        for (var i = 0; i < lodGroupWithUnaprented.Count; ++i)
            if (GUILayout.Button(lodGroupWithUnaprented[i].name))
            {
                Selection.activeGameObject = lodGroupWithUnaprented[i];
                EditorGUIUtility.PingObject(lodGroupWithUnaprented[i]);
            }

        EditorGUILayout.EndScrollView();
    }

    [MenuItem("Tools/FilterLOD")]
    private static void Filter()
    {
        GetWindow<FilterLOD>();
    }

    private void HierarchicalDown(GameObject parent)
    {
        var grp = parent.GetComponent<LODGroup>();

        if (grp != null)
        {
            var expectMeshRenderer = 0;
            for (var i = 0; i < grp.lodCount; ++i) expectMeshRenderer += grp.GetLODs()[i].renderers.Length;

            var actualRenderer = 0;
            foreach (Transform t in grp.transform)
                if (t.GetComponent<Renderer>() != null)
                    actualRenderer += 1;

            if (expectMeshRenderer != actualRenderer) lodGroupWithUnaprented.Add(parent);
        }
        else
        {
            for (var i = 0; i < parent.transform.childCount; ++i)
                HierarchicalDown(parent.transform.GetChild(i).gameObject);
        }
    }
}