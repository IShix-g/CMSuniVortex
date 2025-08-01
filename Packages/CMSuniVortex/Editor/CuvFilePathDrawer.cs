
using System;
using UnityEngine;
using UnityEditor;

namespace CMSuniVortex.Editor
{
    [CustomPropertyDrawer(typeof(CuvFilePathAttribute))]
    internal sealed class CuvFilePathDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.PropertyField(position, property, label, true);
                return;
            }
    
            var extension = (attribute as CuvFilePathAttribute)?.Extension;

            EditorGUI.BeginProperty(position, label, property);

            var buttonRect = new Rect(position.x + position.width - 60, position.y, 60, position.height);
            var fieldRect = new Rect(position.x, position.y, position.width - 65, position.height);

            EditorGUI.PropertyField(fieldRect, property, label);
            if (GUI.Button(buttonRect, "Select"))
            {
                EditorApplication.delayCall += () => 
                {
                    var selectedPath = EditorUtility.OpenFilePanel("Select File Path", 
                        string.IsNullOrEmpty(property.stringValue) ? "Assets/" : property.stringValue, extension); 
            
                    if (!string.IsNullOrEmpty(selectedPath))
                    {
                        var assetsIndex = selectedPath.IndexOf("Assets", StringComparison.Ordinal);
                        if (assetsIndex >= 0)
                        {
                            property.stringValue = selectedPath.Substring(assetsIndex);
                        }
                        else
                        {
                            property.stringValue = string.Empty;
                            Debug.LogError("Please select the path under Assets.");
                        }
                    }
                    property.serializedObject.ApplyModifiedProperties();
                };
            }

            EditorGUI.EndProperty();
        }
    }
}