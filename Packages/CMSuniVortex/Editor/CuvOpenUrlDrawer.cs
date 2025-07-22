
using UnityEngine;
using UnityEditor;

namespace CMSuniVortex.Editor
{
    [CustomPropertyDrawer(typeof(CuvOpenUrlAttribute))]
    internal sealed class CuvOpenUrlDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.PropertyField(position, property, label, true);
                return;
            }
            
            EditorGUI.BeginProperty(position, label, property);
            var buttonRect = new Rect(position.x + position.width - 60, position.y, 60, position.height);
            var fieldRect = new Rect(position.x, position.y, position.width - 65, position.height);

            EditorGUI.PropertyField(fieldRect, property, label);
            EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(property.stringValue));
            if (GUI.Button(buttonRect, "Open"))
            {
                Application.OpenURL(property.stringValue);
            }
            EditorGUI.EndDisabledGroup();
            EditorGUI.EndProperty();
        }
    }
}