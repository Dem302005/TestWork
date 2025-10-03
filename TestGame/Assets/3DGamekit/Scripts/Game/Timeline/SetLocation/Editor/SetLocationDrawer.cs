using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SetLocationBehaviour))]
public class SetLocationDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var fieldCount = 2;
        return fieldCount * EditorGUIUtility.singleLineHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var positionProp = property.FindPropertyRelative("position");
        var eulerAnglesProp = property.FindPropertyRelative("eulerAngles");

        var singleFieldRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        EditorGUI.PropertyField(singleFieldRect, positionProp);

        singleFieldRect.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(singleFieldRect, eulerAnglesProp);
    }
}