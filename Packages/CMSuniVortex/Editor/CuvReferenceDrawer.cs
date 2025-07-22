
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace CMSuniVortex.Editor
{
    [CustomPropertyDrawer(typeof(CuvReferenceAttribute))]
    internal sealed class CuvReferenceDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (Application.isPlaying
                || property.propertyType != SerializedPropertyType.ObjectReference)
            {
                EditorGUI.PropertyField(position, property, label);
                return;
            }
            
            var targetObject = property.serializedObject.targetObject;
            var value = fieldInfo.GetValue(targetObject) as ScriptableObject;

            if (value != default)
            {
                EditorGUI.PropertyField(position, property, label, true);
                return;
            }
            
            var objs = AssetDatabase.FindAssets("t:" + typeof(ScriptableObject))
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<ScriptableObject>)
                .Where(x => x is ICuvKeyReference)
                .ToList();
            
            if (objs.Count == 0)
            {
                EditorGUI.PropertyField(position, property, label);
                return;
            }

            property.serializedObject.UpdateIfRequiredOrScript();
            
            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            var names = objs.Select(obj => obj.name).ToList();
            names.Insert(0, "Select..");
            var optionsArray = names.Select(o => new GUIContent(o)).ToArray();
            var popUpLabel = EditorGUI.BeginProperty(Rect.zero, null, property);
            var currentIndex = Mathf.Clamp(objs.IndexOf(value), 0, objs.Count - 1);
            var newIndex = EditorGUI.Popup(position, popUpLabel, currentIndex, optionsArray);
            var newValue = IsIndexValid(objs, newIndex - 1) ? objs[newIndex - 1] : objs[0];
        
            if (currentIndex != newIndex)
            {
                fieldInfo.SetValue(targetObject, newValue);
            }
            
            EditorGUI.EndProperty();
            
            property.serializedObject.ApplyModifiedProperties();
        }
    
        bool IsIndexValid<T> (List<T> list, int index) => list.Count > 0 && index >= 0 && index < list.Count;
    }
}