
using UnityEditor;
using UnityEngine;

namespace CMSuniVortex.Editor
{
    [CustomPropertyDrawer(typeof(CuvLanguageState))]
    public class CuvLanguageStateDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var isActiveProp = property.FindPropertyRelative("_isActive");
            var languageProp = property.FindPropertyRelative("_language");

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            
            var boolWidth = 25f;
            var spacing = 5f;
            var stringWidth = position.width - boolWidth - spacing;
            var boolRect = new Rect(position.x, position.y, boolWidth, position.height);
            var stringRect = new Rect(position.x + boolWidth + spacing, position.y, stringWidth, position.height);

            EditorGUI.PropertyField(boolRect, isActiveProp, GUIContent.none);
            EditorGUI.PropertyField(stringRect, languageProp, GUIContent.none);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            => EditorGUIUtility.singleLineHeight;
    }

}