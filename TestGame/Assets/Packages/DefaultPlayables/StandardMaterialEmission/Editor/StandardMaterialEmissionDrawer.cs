using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(StandardMaterialEmissionBehaviour))]
public class StandardMaterialEmissionDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var fieldCount = 2;
        return fieldCount * EditorGUIUtility.singleLineHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var ColorProp = property.FindPropertyRelative("color");

        var singleFieldRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        EditorGUI.PropertyField(singleFieldRect, ColorProp);
    }
}