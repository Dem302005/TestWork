using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(AudioSnapshotBehaviour))]
public class AudioSnapshotDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var fieldCount = 5;
        return fieldCount * EditorGUIUtility.singleLineHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var audioClipProp = property.FindPropertyRelative("audioClip");
        var snapshotProp = property.FindPropertyRelative("snapshot");
        var volumeProp = property.FindPropertyRelative("volume");
        var weightedVolumeProp = property.FindPropertyRelative("weightedVolume");
        var audioPlayModeProp = property.FindPropertyRelative("audioPlayMode");

        var singleFieldRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        EditorGUI.PropertyField(singleFieldRect, audioClipProp);

        singleFieldRect.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(singleFieldRect, snapshotProp);

        singleFieldRect.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(singleFieldRect, volumeProp);

        singleFieldRect.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(singleFieldRect, weightedVolumeProp);

        singleFieldRect.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(singleFieldRect, audioPlayModeProp);
    }
}