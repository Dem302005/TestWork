using UnityEditor;
using UnityEngine;

namespace Gamekit3D
{
    [CustomEditor(typeof(CameraSettings))]
    public class CameraSettingsEditor : Editor
    {
        private readonly GUIContent m_AllowRuntimeCameraSettingsChangesContent = new GUIContent(
            "Allow Runtime Camera Settings Changes",
            "When checked this makes it possible to change the Camera Settings' fields while the game is playing in order to test out what feels nice.");

        private SerializedProperty m_AllowRuntimeCameraSettingsChangesProp;

        private readonly GUIContent m_ControllerCameraContent = new GUIContent("Controller Camera",
            "Used to control the camera position when the controller is being used as input.");

        private SerializedProperty m_ControllerCameraProp;

        private readonly GUIContent m_ControllerInvertSettingsContent = new GUIContent("Controller Invert Settings",
            "How the camera will respond to input in the X and Y axes when the Input Choice is set to Controller.");

        private readonly GUIContent m_ControllerInvertSettingsInvertXContent = new GUIContent("Invert X");
        private SerializedProperty m_ControllerInvertSettingsInvertXProp;
        private readonly GUIContent m_ControllerInvertSettingsInvertYContent = new GUIContent("Invert Y");
        private SerializedProperty m_ControllerInvertSettingsInvertYProp;
        private SerializedProperty m_ControllerInvertSettingsProp;

        private readonly GUIContent m_FollowContent = new GUIContent("Follow",
            "Used to determine how the cameras move.  It should be set to Ellen.");

        private SerializedProperty m_FollowProp;

        private readonly GUIContent m_InputChoiceContent = new GUIContent("Input Choice",
            "How you wish to control the player - using a keyboard and mouse or a controller.  Selecting each will change which virtual camera is used.  The virtual cameras differ slightly in order to feel nice for their control method.");

        private SerializedProperty m_InputChoiceProp;

        private readonly GUIContent m_KeyboardAndMouseCameraContent = new GUIContent("Keyboard And Mouse Camera",
            "Used to control the camera position when the keyboard and mouse are being used as input.");

        private SerializedProperty m_KeyboardAndMouseCameraProp;

        private readonly GUIContent m_KeyboardAndMouseInvertSettingsContent = new GUIContent(
            "Keyboard And Mouse Invert Settings",
            "How the camera should respond to input in the X and Y axes when the Input Choice is set to Keyboard and Mouse.");

        private readonly GUIContent m_KeyboardAndMouseInvertSettingsInvertXContent = new GUIContent("Invert X");
        private SerializedProperty m_KeyboardAndMouseInvertSettingsInvertXProp;
        private readonly GUIContent m_KeyboardAndMouseInvertSettingsInvertYContent = new GUIContent("Invert Y");
        private SerializedProperty m_KeyboardAndMouseInvertSettingsInvertYProp;
        private SerializedProperty m_KeyboardAndMouseInvertSettingsProp;

        private readonly GUIContent m_LookAtContent = new GUIContent("Look At",
            "Used to determine how the cameras aim.  It should be set to HeadTarget (this is a child within Ellen's hierarchy).");

        private SerializedProperty m_LookAtProp;

        private readonly GUIContent m_ScriptContent = new GUIContent("Script");
        private SerializedProperty m_ScriptProp;

        private void OnEnable()
        {
            m_ScriptProp = serializedObject.FindProperty("m_Script");
            m_FollowProp = serializedObject.FindProperty("follow");
            m_LookAtProp = serializedObject.FindProperty("lookAt");
            m_KeyboardAndMouseCameraProp = serializedObject.FindProperty("keyboardAndMouseCamera");
            m_ControllerCameraProp = serializedObject.FindProperty("controllerCamera");
            m_InputChoiceProp = serializedObject.FindProperty("inputChoice");
            m_KeyboardAndMouseInvertSettingsProp = serializedObject.FindProperty("keyboardAndMouseInvertSettings");
            m_KeyboardAndMouseInvertSettingsInvertXProp =
                m_KeyboardAndMouseInvertSettingsProp.FindPropertyRelative("invertX");
            m_KeyboardAndMouseInvertSettingsInvertYProp =
                m_KeyboardAndMouseInvertSettingsProp.FindPropertyRelative("invertY");
            m_ControllerInvertSettingsProp = serializedObject.FindProperty("controllerInvertSettings");
            m_ControllerInvertSettingsInvertXProp = m_ControllerInvertSettingsProp.FindPropertyRelative("invertX");
            m_ControllerInvertSettingsInvertYProp = m_ControllerInvertSettingsProp.FindPropertyRelative("invertY");
            m_AllowRuntimeCameraSettingsChangesProp =
                serializedObject.FindProperty("allowRuntimeCameraSettingsChanges");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GUI.enabled = false;
            EditorGUILayout.PropertyField(m_ScriptProp, m_ScriptContent);
            GUI.enabled = true;

            EditorGUILayout.PropertyField(m_FollowProp, m_FollowContent);
            EditorGUILayout.PropertyField(m_LookAtProp, m_LookAtContent);
            EditorGUILayout.PropertyField(m_KeyboardAndMouseCameraProp, m_KeyboardAndMouseCameraContent);
            EditorGUILayout.PropertyField(m_ControllerCameraProp, m_ControllerCameraContent);
            EditorGUILayout.PropertyField(m_InputChoiceProp, m_InputChoiceContent);

            GUI.enabled = m_InputChoiceProp.intValue == 0;
            EditorGUILayout.LabelField(m_KeyboardAndMouseInvertSettingsContent);
            EditorGUI.indentLevel++;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(m_KeyboardAndMouseInvertSettingsInvertXProp,
                m_KeyboardAndMouseInvertSettingsInvertXContent);
            EditorGUILayout.PropertyField(m_KeyboardAndMouseInvertSettingsInvertYProp,
                m_KeyboardAndMouseInvertSettingsInvertYContent);
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel--;
            GUI.enabled = m_InputChoiceProp.intValue == 1;
            EditorGUILayout.LabelField(m_ControllerInvertSettingsContent);
            EditorGUI.indentLevel++;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(m_ControllerInvertSettingsInvertXProp,
                m_ControllerInvertSettingsInvertXContent);
            EditorGUILayout.PropertyField(m_ControllerInvertSettingsInvertYProp,
                m_ControllerInvertSettingsInvertYContent);
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel--;
            GUI.enabled = true;

            EditorGUILayout.PropertyField(m_AllowRuntimeCameraSettingsChangesProp,
                m_AllowRuntimeCameraSettingsChangesContent);

            serializedObject.ApplyModifiedProperties();
        }
    }
}