
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;

namespace CMSuniVortex.Editor
{
    [CustomPropertyDrawer(typeof(CuvAllLanguageAttribute))]
    public sealed class CuvLanguagesPropertyDrawer : PropertyDrawer
    {
        readonly List<string> _objs = new ();

        static IReadOnlyList<ICuvLocalizedClient> s_localizedClients;
        
        static List<string> s_languageStrings;
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                var errorStyle = new GUIStyle(EditorStyles.label)
                {
                    normal = { textColor = Color.red },
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleLeft,
                };

                EditorGUI.LabelField(position, "[" + typeof(CuvAllLanguageAttribute).Name + "] Please specify a string.", errorStyle);
                return;
            }

            if (Application.isPlaying)
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUI.PropertyField(position, property, label);
                EditorGUI.EndDisabledGroup();
                return;
            }

            s_localizedClients ??= CuvImportersCache.FilterClients<ICuvLocalizedClient>();
            s_languageStrings ??= GetAllLanguages(s_localizedClients);
            var isDefaultApplicationLanguage = ((CuvAllLanguageAttribute) attribute).IsDefaultApplicationLanguage;
            
            EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            var value = property.stringValue;
            _objs.Clear();
            if (isDefaultApplicationLanguage)
            {
                _objs.Add(CuvLanguageSettings.AppLangName);
            }
            else
            {
                _objs.Add("Select..");
            }
            _objs.AddRange(s_languageStrings);
            
            var optionsArray = _objs.Select(o => new GUIContent(o)).ToArray();
            var currentIndex = Mathf.Clamp(_objs.IndexOf(value), 0, _objs.Count - 1);
            var newIndex = EditorGUI.Popup(position, currentIndex, optionsArray);
            var newValue = IsIndexValid(_objs, newIndex) ? _objs[newIndex] : _objs[0];
            if (currentIndex != newIndex)
            {
                property.stringValue = !isDefaultApplicationLanguage || newIndex > 0
                    ? newValue
                    : string.Empty;
            }
            EditorGUI.EndProperty();
        }
        
        bool IsIndexValid<T>(List<T> list, int index)
            => list.Count > 0 && index >= 0 && index < list.Count;

        static List<string> GetAllLanguages(IReadOnlyList<ICuvLocalizedClient> selectables)
        {
            var results = new List<string>();
            var hashSet = HashSetPool<SystemLanguage>.Get();
            try
            {
                foreach (var selectable in selectables)
                {
                    hashSet.UnionWith(selectable.GetLanguages());
                }

                foreach (var language in hashSet.OrderBy(x => x.ToString()))
                {
                    results.Add(language.ToString());
                }
            }
            finally
            {
                HashSetPool<SystemLanguage>.Release(hashSet);
            }

            return results;
        }
    }
}