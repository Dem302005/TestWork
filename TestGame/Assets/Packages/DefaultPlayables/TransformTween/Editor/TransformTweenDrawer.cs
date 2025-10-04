using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(TransformTweenBehaviour))]
public class TransformTweenDrawer : PropertyDrawer
{
    private readonly GUIContent m_EndingSpeedContent = new GUIContent("Ending Speed",
        "This is used when the Tween Type is set to Custom.  It determines how fast the transform will be moving near the End Location.");

    private readonly GUIContent m_StartingSpeedContent = new GUIContent("Starting Speed",
        "This is used when the Tween Type is set to Custom.  It determines how fast the transform will be moving near the Start Location.");

    private readonly GUIContent m_TweenPositionContent = new GUIContent("Tween Position",
        "This should be true if the transformToMove to change position.  This causes recalulations each frame which are more CPU intensive.");

    private readonly GUIContent m_TweenRotationContent = new GUIContent("Tween Rotation",
        "This should be true if the transformToMove to change rotation.");

    private readonly GUIContent m_TweenTypeContent = new GUIContent("Tween Type",
        "Linear - the transform moves the same amount each frame (assuming static start and end locations).\n"
        + "Deceleration - the transform moves slower the closer to the end location it is.\n"
        + "Harmonic - the transform moves faster in the middle of its tween.\n"
        + "Custom - uses the customStartingSpeed and customEndingSpeed to create a curve for the desired tween.");

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var fieldCount = property.FindPropertyRelative("tweenType").enumValueIndex ==
                         (int)TransformTweenBehaviour.TweenType.Custom
            ? 5
            : 3;
        return fieldCount * EditorGUIUtility.singleLineHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var tweenPositionProp = property.FindPropertyRelative("tweenPosition");
        var tweenRotationProp = property.FindPropertyRelative("tweenRotation");
        var tweenTypeProp = property.FindPropertyRelative("tweenType");

        var singleFieldRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        EditorGUI.PropertyField(singleFieldRect, tweenPositionProp, m_TweenPositionContent);

        singleFieldRect.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(singleFieldRect, tweenRotationProp, m_TweenRotationContent);

        singleFieldRect.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(singleFieldRect, tweenTypeProp, m_TweenTypeContent);

        if (tweenTypeProp.enumValueIndex == (int)TransformTweenBehaviour.TweenType.Custom)
        {
            var startingSpeedProp = property.FindPropertyRelative("customStartingSpeed");
            var endingSpeedProp = property.FindPropertyRelative("customEndingSpeed");

            singleFieldRect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.Slider(singleFieldRect, startingSpeedProp, 0f, 1f, m_StartingSpeedContent);

            singleFieldRect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.Slider(singleFieldRect, endingSpeedProp, 0f, 1f, m_EndingSpeedContent);
        }
    }
}