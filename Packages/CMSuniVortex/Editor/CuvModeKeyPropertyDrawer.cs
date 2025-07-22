
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CMSuniVortex.Editor
{
    [CustomPropertyDrawer(typeof(CuvModelKeyAttribute))]
    internal sealed class CuvModeKeyPropertyDrawer : PropertyDrawer
    {
        readonly List<string> _objs = new ();
        
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
            
            EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            var value = property.stringValue;
            _objs.Clear();
            _objs.Add("Select..");
            if(referenceProp is {objectReferenceValue: ICuvKeyReference reference})
            {
                _objs.AddRange(reference.GetKeys().ToList());
            }
            
            var optionsArray = _objs.Select(o => new GUIContent(o)).ToArray();
            var currentIndex = Mathf.Clamp(_objs.IndexOf(value), 0, _objs.Count - 1);
            var newIndex = EditorGUI.Popup(position, currentIndex, optionsArray);
            var newValue = IsIndexValid(_objs, newIndex) ? _objs[newIndex] : _objs[0];
            if (currentIndex != newIndex)
            {
                property.stringValue = newValue;
            }
            EditorGUI.EndProperty();
        }
        
        bool IsIndexValid<T>(List<T> list, int index)
            => list.Count > 0 && index >= 0 && index < list.Count;
    }
}