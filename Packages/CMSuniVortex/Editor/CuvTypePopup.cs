
using System;
using UnityEditor;
using UnityEngine;

namespace CMSuniVortex.Editor
{
    sealed class CuvTypePopup
    {
        public SerializedProperty Property { get; private set; }
        public Type[] Types { get; private set; }
        public int SelectedIndex { get; set; }

        string[] _options;
        
        public CuvTypePopup(SerializedProperty property, Type[] types)
        {
            Property = property;
            Types = Array.Empty<Type>();
            ResetTypes(types);
        }

        public void ResetTypes(Type[] types)
        {
            Types = types;
            
            _options = new string[Types.Length + 1];
            _options[0] = Types.Length > 0 ? "Select.." : "No Available Types";
            for (var i = 0; i < Types.Length; i++)
            {
                var type = Types[i];
                var className = (Attribute.IsDefined(type, typeof(CuvDisplayNameAttribute)))
                    ? ((CuvDisplayNameAttribute)Attribute.GetCustomAttribute(type, typeof(CuvDisplayNameAttribute))).DisplayName
                    : type.FullName;
                _options[i + 1] = className;
            }
        }

        public void ResetReference()
        {
            Property.managedReferenceValue = default;
            SelectedIndex = 0;
        }
        
        public bool Draw()
        {
            EditorGUI.BeginChangeCheck();
            var fullTypeName = Property.managedReferenceFullTypename;
            var targetIndex = string.IsNullOrEmpty(fullTypeName) || fullTypeName == "  " ? 0 : Array.FindIndex(Types, t => t.FullName == fullTypeName.Substring(fullTypeName.IndexOf(' ') + 1)) + 1;
            var selectedIndex = EditorGUILayout.Popup(targetIndex == 0 ? Property.displayName : string.Empty, targetIndex, _options);
            
            SelectedIndex = selectedIndex;

            if (!EditorGUI.EndChangeCheck())
            {
                if (Property.managedReferenceValue != default)
                {
                    EditorGUILayout.PropertyField(Property, true);
                }
                return false;
            }
            if (selectedIndex == 0)
            {
                Property.managedReferenceValue = default;
            }
            else
            {
                GUILayout.Space(5);
                var selectedType = Types[selectedIndex - 1];
                if (fullTypeName != selectedType.Assembly.GetName().Name + " " + selectedType.FullName)
                {
                    Property.managedReferenceValue = Activator.CreateInstance(selectedType);
                }
            }
            return true;
        }
    }
}