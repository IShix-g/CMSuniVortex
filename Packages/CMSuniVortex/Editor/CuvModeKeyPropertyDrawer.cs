
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CMSuniVortex.Editor
{
    [CustomPropertyDrawer(typeof(CuvModelKeyAttribute))]
    sealed class CuvModeKeyPropertyDrawer : PropertyDrawer
    {
        static readonly List<string> s_emptyList = new ();
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.PropertyField(position, property, label);
                return;
            }
            if (Application.isPlaying)
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUI.PropertyField(position, property, label);
                EditorGUI.EndDisabledGroup();
                return;
            }
            
            var referenceName = (attribute as CuvModelKeyAttribute)?.ReferenceName;
            var serializedObject = property.serializedObject;
            var referenceProp = default(SerializedProperty);

            if (!string.IsNullOrEmpty(referenceName))
            {
                referenceProp = serializedObject.FindProperty(referenceName);
            }
            
            if (!Application.isPlaying)
            {
                EditorGUI.BeginProperty(position, label, property);
                position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
                var value = property.stringValue;
                var objs = GetKeys(referenceProp);
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

        List<string> GetKeys(SerializedProperty prop)
        {
            return prop switch
            {
                {objectReferenceValue: ICuvReference reference} => reference.GetKeys().ToList(),
                {objectReferenceValue: ICuvAsyncReference asyncReference} => asyncReference.GetKeys().ToList(),
                _ => s_emptyList
            };
        }
        
        bool IsIndexValid<T>(List<T> list, int index)
            => list.Count > 0 && index >= 0 && index < list.Count;
    }
}