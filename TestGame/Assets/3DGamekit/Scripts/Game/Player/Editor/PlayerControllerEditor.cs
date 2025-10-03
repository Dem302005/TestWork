using UnityEditor;
using UnityEngine;

namespace Gamekit3D
{
    [CustomEditor(typeof(PlayerController))]
    public class PlayerControllerEditor : Editor
    {
        private readonly GUIContent m_CameraSettingsContent = new GUIContent("Camera Settings",
            "Used to get the rotation of the current camera so that Ellen faces the correct direction.  Note: This is the only reference which is not part of the Ellen prefab.  It should automatically be set to the Camera Settings script of the CameraRig gameobject when the Prefab is instantiated.");

        private SerializedProperty m_CameraSettingsProp;

        private readonly GUIContent m_CanAttackContent = new GUIContent("Can Attack",
            "Whether or not Ellen can attack with her staff.  This can be set externally.");

        private SerializedProperty m_CanAttackProp;

        private readonly GUIContent m_EmoteAttackPlayerContent = new GUIContent("Emote Attack Player",
            "Used to play a random vocal sound when Ellen attacks.");

        private SerializedProperty m_EmoteAttackPlayerProp;

        private readonly GUIContent m_EmoteDeathPlayerContent =
            new GUIContent("Emote Death Player", "Used to play a random vocal sound when Ellen dies.");

        private SerializedProperty m_EmoteDeathPlayerProp;

        private readonly GUIContent m_EmoteJumpPlayerContent =
            new GUIContent("Emote Jump Player", "Used to play a random vocal sound when Ellen jumps.");

        private SerializedProperty m_EmoteJumpPlayerProp;

        private readonly GUIContent m_EmoteLandingPlayerContent = new GUIContent("Emote Landing Player",
            "Used to play a random vocal sound when Ellen lands.");

        private SerializedProperty m_EmoteLandingPlayerProp;

        private readonly GUIContent m_FootstepPlayerContent = new GUIContent("Footstep Random Audio Player",
            "Used to play a random sound when Ellen takes a step.");

        private SerializedProperty m_FootstepPlayerProp;

        private readonly GUIContent m_GravityContent =
            new GUIContent("Gravity", "How fast Ellen falls when in the air.");

        private SerializedProperty m_GravityProp;

        private readonly GUIContent m_HurtAudioPlayerContent = new GUIContent("Hurt Random Audio Player",
            "Used to play a random sound when Ellen gets hurt.");

        private SerializedProperty m_HurtAudioPlayerProp;

        private readonly GUIContent m_IdleTimeoutContent = new GUIContent("Idle Timeout",
            "How many seconds before Ellen starts considering random Idle poses.");

        private SerializedProperty m_IdleTimeoutProp;

        private readonly GUIContent m_JumpSpeedContent =
            new GUIContent("Jump Speed", "How fast Ellen takes off when jumping.");

        private SerializedProperty m_JumpSpeedProp;

        private readonly GUIContent m_LandingPlayerContent = new GUIContent("Landing Random Audio Player",
            "Used to play a random sound when Ellen lands.");

        private SerializedProperty m_LandingPlayerProp;

        private readonly GUIContent m_MaxForwardSpeedContent =
            new GUIContent("Max Forward Speed", "How fast Ellen can run.");

        private SerializedProperty m_MaxForwardSpeedProp;
        private SerializedProperty m_MaxTurnSpeedProp;

        private readonly GUIContent m_MeleeWeaponContent =
            new GUIContent("Melee Weapon", "Used for damaging enemies when Ellen swings her staff.");

        private SerializedProperty m_MeleeWeaponProp;
        private SerializedProperty m_MinTurnSpeedProp;

        private readonly GUIContent m_ScriptContent = new GUIContent("Script");
        private SerializedProperty m_ScriptProp;

        private readonly GUIContent m_TurnSpeedContent = new GUIContent("Turn Speed",
            "How fast Ellen turns.  This varies depending on how fast she is moving.  When stationary the maximum will be used and when running at Max Forward Speed the minimum will be used.");

        private void OnEnable()
        {
            m_ScriptProp = serializedObject.FindProperty("m_Script");

            m_MaxForwardSpeedProp = serializedObject.FindProperty("maxForwardSpeed");
            m_GravityProp = serializedObject.FindProperty("gravity");
            m_JumpSpeedProp = serializedObject.FindProperty("jumpSpeed");
            m_MinTurnSpeedProp = serializedObject.FindProperty("minTurnSpeed");
            m_MaxTurnSpeedProp = serializedObject.FindProperty("maxTurnSpeed");
            m_IdleTimeoutProp = serializedObject.FindProperty("idleTimeout");
            m_CanAttackProp = serializedObject.FindProperty("canAttack");

            m_MeleeWeaponProp = serializedObject.FindProperty("meleeWeapon");
            m_CameraSettingsProp = serializedObject.FindProperty("cameraSettings");
            m_FootstepPlayerProp = serializedObject.FindProperty("footstepPlayer");
            m_HurtAudioPlayerProp = serializedObject.FindProperty("hurtAudioPlayer");
            m_LandingPlayerProp = serializedObject.FindProperty("landingPlayer");
            m_EmoteLandingPlayerProp = serializedObject.FindProperty("emoteLandingPlayer");
            m_EmoteDeathPlayerProp = serializedObject.FindProperty("emoteDeathPlayer");
            m_EmoteAttackPlayerProp = serializedObject.FindProperty("emoteAttackPlayer");
            m_EmoteJumpPlayerProp = serializedObject.FindProperty("emoteJumpPlayer");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GUI.enabled = false;
            EditorGUILayout.PropertyField(m_ScriptProp, m_ScriptContent);
            GUI.enabled = true;

            m_MaxForwardSpeedProp.floatValue =
                EditorGUILayout.Slider(m_MaxForwardSpeedContent, m_MaxForwardSpeedProp.floatValue, 4f, 12f);
            m_GravityProp.floatValue = EditorGUILayout.Slider(m_GravityContent, m_GravityProp.floatValue, 10f, 30f);
            m_JumpSpeedProp.floatValue =
                EditorGUILayout.Slider(m_JumpSpeedContent, m_JumpSpeedProp.floatValue, 5f, 20f);

            MinMaxTurnSpeed();

            EditorGUILayout.PropertyField(m_IdleTimeoutProp, m_IdleTimeoutContent);
            EditorGUILayout.PropertyField(m_CanAttackProp, m_CanAttackContent);

            EditorGUILayout.Space();

            m_MeleeWeaponProp.isExpanded = EditorGUILayout.Foldout(m_MeleeWeaponProp.isExpanded, "References");

            if (m_MeleeWeaponProp.isExpanded)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(m_MeleeWeaponProp, m_MeleeWeaponContent);
                EditorGUILayout.PropertyField(m_CameraSettingsProp, m_CameraSettingsContent);
                EditorGUILayout.PropertyField(m_FootstepPlayerProp, m_FootstepPlayerContent);
                EditorGUILayout.PropertyField(m_HurtAudioPlayerProp, m_HurtAudioPlayerContent);
                EditorGUILayout.PropertyField(m_LandingPlayerProp, m_LandingPlayerContent);
                EditorGUILayout.PropertyField(m_EmoteLandingPlayerProp, m_EmoteLandingPlayerContent);
                EditorGUILayout.PropertyField(m_EmoteDeathPlayerProp, m_EmoteDeathPlayerContent);
                EditorGUILayout.PropertyField(m_EmoteAttackPlayerProp, m_EmoteAttackPlayerContent);
                EditorGUILayout.PropertyField(m_EmoteJumpPlayerProp, m_EmoteJumpPlayerContent);
                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void MinMaxTurnSpeed()
        {
            var position = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight);

            const float spacing = 4f;
            const float intFieldWidth = 50f;

            position.width -= spacing * 3f + intFieldWidth * 2f;

            var labelRect = position;
            labelRect.width *= 0.48f;

            var minRect = position;
            minRect.width = 50f;
            minRect.x += labelRect.width + spacing;

            var sliderRect = position;
            sliderRect.width *= 0.52f;
            sliderRect.x += labelRect.width + minRect.width + spacing * 2f;

            var maxRect = position;
            maxRect.width = minRect.width;
            maxRect.x += labelRect.width + minRect.width + sliderRect.width + spacing * 3f;

            EditorGUI.LabelField(labelRect, m_TurnSpeedContent);
            m_MinTurnSpeedProp.floatValue = EditorGUI.IntField(minRect, (int)m_MinTurnSpeedProp.floatValue);

            var minTurnSpeed = m_MinTurnSpeedProp.floatValue;
            var maxTurnSpeed = m_MaxTurnSpeedProp.floatValue;
            EditorGUI.MinMaxSlider(sliderRect, GUIContent.none, ref minTurnSpeed, ref maxTurnSpeed, 100f, 1500f);
            m_MinTurnSpeedProp.floatValue = minTurnSpeed;
            m_MaxTurnSpeedProp.floatValue = maxTurnSpeed;

            m_MaxTurnSpeedProp.floatValue = EditorGUI.IntField(maxRect, (int)m_MaxTurnSpeedProp.floatValue);
        }
    }
}