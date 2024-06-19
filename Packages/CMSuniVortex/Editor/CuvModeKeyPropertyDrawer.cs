
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CMSuniVortex.Editor
{
    [CustomPropertyDrawer(typeof(CuvModeKeyAttribute))]
    public class CuvModeKeyPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.PropertyField(position, property, label, true);
                return;
            }
            
            var referenceName = (attribute as CuvModeKeyAttribute)?.ReferenceName;
            var serializedObject = property.serializedObject;
            var referenceProp = default(SerializedProperty);

            if (!string.IsNullOrEmpty(referenceName))
            {
                referenceProp = serializedObject.FindProperty(referenceName);
            }
            
            if (referenceProp is {objectReferenceValue: ICuvReference reference})
            {
                EditorGUI.BeginProperty(position, label, property);
                position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
                
                var value = property.stringValue;
                var objs = reference.GetKeys().ToList();
                objs.Insert(0, "Select..");
                var optionsArray = objs.Select(o => new GUIContent(o)).ToArray();
                var currentIndex = Mathf.Clamp(objs.IndexOf(value), 0, objs.Count - 1);
                var newIndex = EditorGUI.Popup(position, currentIndex, optionsArray);
                var newValue = IsIndexValid(objs, newIndex) ? objs[newIndex] : objs[0];
                if (currentIndex != newIndex)
                {
                    property.stringValue = newValue;
                }
                EditorGUI.EndProperty();
            }
            else
            {
                EditorGUI.LabelField(position, "Set the Reference.");
            }
        }

        bool IsIndexValid<T>(List<T> list, int index)
            => list.Count > 0 && index >= 0 && index < list.Count;
    }
}