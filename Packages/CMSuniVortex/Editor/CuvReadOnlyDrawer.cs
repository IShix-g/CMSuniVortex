
using UnityEngine;
using UnityEditor;

namespace CMSuniVortex.Editor
{
    [CustomPropertyDrawer(typeof(CuvReadOnlyAttribute))]
    internal sealed class CuvReadOnlyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var wasEnabled = GUI.enabled;
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = wasEnabled;
        }
    }
}