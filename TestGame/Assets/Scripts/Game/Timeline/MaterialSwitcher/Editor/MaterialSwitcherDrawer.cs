using UnityEditor;
using UnityEngine;

//[CustomPropertyDrawer(typeof(MaterialSwitcherBehaviour))]
// NOT WORKING, DO NOT ENABLE
public class MaterialSwitcherDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var materialIndexPairsProp = property.FindPropertyRelative("materialIndexPairs");
        var fieldCount = materialIndexPairsProp.isExpanded ? 2 : 1;
        fieldCount += 2 * materialIndexPairsProp.arraySize;
        return fieldCount * EditorGUIUtility.singleLineHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var materialIndexPairsProp = property.FindPropertyRelative("materialIndexPairs");

        var singleFieldRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        EditorGUI.PropertyField(singleFieldRect, materialIndexPairsProp);

        singleFieldRect.y += EditorGUIUtility.singleLineHeight;
        materialIndexPairsProp.arraySize =
            EditorGUI.IntField(singleFieldRect, "size", materialIndexPairsProp.arraySize);

        EditorGUI.indentLevel++;
        for (var i = 0; i < materialIndexPairsProp.arraySize; i++)
        {
            var pairProp = materialIndexPairsProp.GetArrayElementAtIndex(i);
            var materialProp = pairProp.FindPropertyRelative("replacementMaterial");
            var indexProp = pairProp.FindPropertyRelative("materialIndexToReplace");

            singleFieldRect.y += 5f;
            singleFieldRect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(singleFieldRect, materialProp);

            singleFieldRect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(singleFieldRect, indexProp);
        }

        EditorGUI.indentLevel--;
    }
}