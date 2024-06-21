
using System;
using UnityEngine;
using UnityEditor;

namespace CMSuniVortex.Editor
{
    [CustomPropertyDrawer(typeof(CuvFilePathAttribute))]
    sealed class CuvFilePathDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
        {
            if (prop.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.PropertyField(position, prop, label, true);
                return;
            }
    
            var extension = (attribute as CuvFilePathAttribute)?.Extension;

            EditorGUI.BeginProperty(position, label, prop);
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            var buttonRect = new Rect(position.x + position.width - 60, position.y, 60, position.height);
            var fieldRect = new Rect(position.x, position.y, position.width - 65, position.height);

            EditorGUI.PropertyField(fieldRect, prop, GUIContent.none);
            if (GUI.Button(buttonRect, "Select"))
            {
                EditorApplication.delayCall += () => 
                {
                    var selectedPath = EditorUtility.OpenFilePanel("Select File Path", 
                        string.IsNullOrEmpty(prop.stringValue) ? "Assets/" : prop.stringValue, extension); 
            
                    if (!string.IsNullOrEmpty(selectedPath))
                    {
                        var assetsIndex = selectedPath.IndexOf("Assets", StringComparison.Ordinal);
                        if (assetsIndex >= 0)
                        {
                            prop.stringValue = selectedPath.Substring(assetsIndex);
                        }
                        else
                        {
                            prop.stringValue = string.Empty;
                            Debug.LogError("Please select the path under Assets.");
                        }
                    }
                    prop.serializedObject.ApplyModifiedProperties();
                };
            }

            EditorGUI.EndProperty();
        }
    }
}