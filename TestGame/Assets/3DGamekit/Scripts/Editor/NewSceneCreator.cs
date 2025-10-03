using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gamekit3D
{
    public class NewSceneCreator : EditorWindow
    {
        protected readonly GUIContent m_NameContent = new GUIContent("New Scene Name");
        protected string m_NewSceneName;

        private void OnGUI()
        {
            m_NewSceneName = EditorGUILayout.TextField(m_NameContent, m_NewSceneName);

            GUI.enabled = !string.IsNullOrWhiteSpace(m_NewSceneName);
            if (GUILayout.Button("Create"))
                CheckAndCreateScene();
        }

        [MenuItem("Kit Tools/Create New Scene...", priority = 100)]
        private static void Init()
        {
            var window = GetWindow<NewSceneCreator>();
            window.Show();
        }

        protected void CheckAndCreateScene()
        {
            if (EditorApplication.isPlaying)
            {
                Debug.LogWarning("Cannot create scenes while in play mode.  Exit play mode first.");
                return;
            }

            var currentActiveScene = SceneManager.GetActiveScene();

            if (currentActiveScene.isDirty)
            {
                var title = currentActiveScene.name + " Has Been Modified";
                var message = "Do you want to save the changes you made to " + currentActiveScene.path +
                              "?\nChanges will be lost if you don't save them.";
                var option = EditorUtility.DisplayDialogComplex(title, message, "Save", "Don't Save", "Cancel");

                if (option == 0)
                    EditorSceneManager.SaveScene(currentActiveScene);
                else if (option == 2) return;
            }

            CreateScene();
        }

        protected void CreateScene()
        {
            var result = AssetDatabase.FindAssets("_TemplateScene");

            if (result.Length > 0)
            {
                var originalScenePath = AssetDatabase.GUIDToAssetPath(result[0]) + ".unity";
                var newScenePath = "Assets/" + m_NewSceneName + ".unity";

                if (!AssetDatabase.CopyAsset(originalScenePath, newScenePath))
                {
                    Debug.LogError("Couldn't copy the scene to the new location'");
                    return;
                }

                AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);

                var newScene = EditorSceneManager.OpenScene(newScenePath, OpenSceneMode.Single);
                AddSceneToBuildSettings(newScene);
                Close();
            }
            else
            {
                //Debug.LogError("The template scene <b>_TemplateScene</b> couldn't be found ");
                EditorUtility.DisplayDialog("Error",
                    "The scene _TemplateScene was not found in Gamekit3D/Scenes folder. This scene is required by the New Scene Creator.",
                    "OK");
            }
        }

        protected void AddSceneToBuildSettings(Scene scene)
        {
            var buildScenes = EditorBuildSettings.scenes;

            var newBuildScenes = new EditorBuildSettingsScene[buildScenes.Length + 1];
            for (var i = 0; i < buildScenes.Length; i++) newBuildScenes[i] = buildScenes[i];
            newBuildScenes[buildScenes.Length] = new EditorBuildSettingsScene(scene.path, true);
            EditorBuildSettings.scenes = newBuildScenes;
        }

        protected GameObject InstantiatePrefab(string folderPath, string prefabName)
        {
            GameObject instance = null;
            string[] prefabFolderPath = { folderPath };
            var guids = AssetDatabase.FindAssets(prefabName, prefabFolderPath);

            if (guids.Length == 0)
            {
                Debug.LogError("The " + prefabName + " prefab could not be found in " + folderPath +
                               " and could therefore not be instantiated.  Please create one manually.");
            }
            else if (guids.Length > 1)
            {
                Debug.LogError("Multiple " + prefabName + " prefabs were found in " + folderPath +
                               " and one could therefore not be instantiated.  Please create one manually.");
            }
            else
            {
                var path = AssetDatabase.GUIDToAssetPath(guids[0]);
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                instance = Instantiate(prefab);
            }

            return instance;
        }
    }
}