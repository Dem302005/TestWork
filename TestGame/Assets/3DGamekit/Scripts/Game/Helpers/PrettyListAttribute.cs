using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Gamekit3D
{
    public class EnforceTypeAttribute : PropertyAttribute
    {
        public Type type;

        public EnforceTypeAttribute(Type enforcedType)
        {
            type = enforcedType;
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(EnforceTypeAttribute))]
    public class PrettyListDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var propAttribute = attribute as EnforceTypeAttribute;
            EditorGUI.BeginProperty(position, label, property);

            var obj =
                EditorGUI.ObjectField(position, property.objectReferenceValue, typeof(MonoBehaviour), true) as
                    MonoBehaviour;
            if (obj != null && propAttribute.type.IsAssignableFrom(obj.GetType()) && !EditorGUI.showMixedValue)
                property.objectReferenceValue = obj;
            EditorGUI.EndProperty();
        }
    }
#endif
}