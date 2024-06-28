
using UnityEngine;
using UnityEditor;

namespace CMSuniVortex.Editor
{
    [CustomPropertyDrawer(typeof(CuvOpenUrlAttribute))]
    public sealed class CuvOpenUrlDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
        {
            if (prop.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.PropertyField(position, prop, label, true);
                return;
            }
            
            EditorGUI.BeginProperty(position, label, prop);
            var buttonRect = new Rect(position.x + position.width - 60, position.y, 60, position.height);
            var fieldRect = new Rect(position.x, position.y, position.width - 65, position.height);

            EditorGUI.PropertyField(fieldRect, prop, label);
            EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(prop.stringValue));
            if (GUI.Button(buttonRect, "Open"))
            {
                Application.OpenURL(prop.stringValue);
            }
            EditorGUI.EndDisabledGroup();
            EditorGUI.EndProperty();
        }
    }
}