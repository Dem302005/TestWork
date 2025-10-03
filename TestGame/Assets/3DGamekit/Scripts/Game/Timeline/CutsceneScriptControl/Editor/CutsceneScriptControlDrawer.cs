using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(CutsceneScriptControlBehaviour))]
public class CutsceneScriptControlDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 2f * EditorGUIUtility.singleLineHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var playerInputEnabledProp = property.FindPropertyRelative("playerInputEnabled");

        var singleFieldRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        EditorGUI.PropertyField(singleFieldRect, playerInputEnabledProp);
    }
}