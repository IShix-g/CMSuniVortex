
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;

namespace CMSuniVortex.Editor
{
    internal class CuvLanguageSettingsWindow : EditorWindow
    {
        [MenuItem("Window/CMSuniVortex/open Language setting")]
        static void ShowWindow()
        {
            var window = GetWindow<CuvLanguageSettingsWindow>();
            window.titleContent = new GUIContent("Language setting");
            window.Show();
        }

        ICuvLocalizedClient[] _localizedClients;
        Texture2D _logo;
        SerializedObject _serializedObject;
        SerializedProperty _testLanguageProp;
        SerializedProperty _languageStatusProp;
        SerializedProperty _defaultLanguageProp;
        SerializedProperty _startLanguageProp;
        SerializedProperty _saveLanguageProp;
        GUIContent _refreshIcon;
        GUIContent _searchIcon;
        Vector2 _scrollPos;
        float _importTime;
        float _lastTimeSinceStartup;

        void OnEnable()
        {
            CuvLanguageSettings.CreateAndLoad();
            _logo = CuvImporterEditor.GetLogo();
            _serializedObject = new SerializedObject(CuvLanguageSettings.Instance);
            _testLanguageProp = _serializedObject.FindProperty("_languageTest");
            _languageStatusProp = _serializedObject.FindProperty("_languages");
            _defaultLanguageProp = _serializedObject.FindProperty("_defaultLanguage");
            _startLanguageProp = _serializedObject.FindProperty("_startLanguage");
            _saveLanguageProp = _serializedObject.FindProperty("_saveLanguage");
            _refreshIcon = EditorGUIUtility.IconContent("Refresh");
            _searchIcon = EditorGUIUtility.IconContent("Search Icon");
            ImportLanguages();
        }

        void OnDisable()
        {
            _serializedObject.Dispose();
            _languageStatusProp.Dispose();
            _defaultLanguageProp.Dispose();
            _startLanguageProp.Dispose();
            _saveLanguageProp.Dispose();
        }

        void ImportLanguages()
        {
            CuvImportersCache.ReImport();
            var settings = CuvLanguageSettings.Instance;
            _localizedClients = CuvImportersCache.FilterClients<ICuvLocalizedClient>();
            
            if (string.IsNullOrEmpty(settings.DefaultLanguage))
            {
                foreach (var selectable in _localizedClients)
                {
                    if (selectable.GetLanguages().Length <= 0)
                    {
                        continue;
                    }
                    var language = selectable.GetLanguages().First();
                    settings.SetDefaultLanguage(language);
                    break;
                }
            }

            var hashSet = HashSetPool<SystemLanguage>.Get();
            try
            {
                foreach (var selectable in _localizedClients)
                {
                    hashSet.UnionWith(selectable.GetLanguages());
                }
                settings.AddLanguages(hashSet);
            }
            finally
            {
                HashSetPool<SystemLanguage>.Release(hashSet);
            }
        }
        
        void OnGUI()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                var style = new GUIStyle(GUI.skin.label)
                {
                    padding = new RectOffset(5, 5, 5, 5),
                    alignment = TextAnchor.MiddleCenter,
                };
                GUILayout.Label(_logo, style, GUILayout.MinWidth(430), GUILayout.Height(75));
            }
            {
                var style = new GUIStyle(GUI.skin.label)
                {
                    padding = new RectOffset(5, 5, 0, 10),
                    alignment = TextAnchor.MiddleCenter,
                };
                GUILayout.Label("Language Settings", style, GUILayout.MinWidth(430));
            }
            
            var importText = _importTime > 0
                && _lastTimeSinceStartup > 0
                        ? " Importing..."
                        : " Re-import Language";
            if (_importTime < 0.5f
                && _lastTimeSinceStartup > 0)
            {
                var currentTime = (float) EditorApplication.timeSinceStartup;
                _importTime = currentTime - _lastTimeSinceStartup;
                Repaint();
            }
            else
            {
                _importTime = 0;
                _lastTimeSinceStartup = 0;
            }
            
            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginDisabledGroup(
                _importTime < 1f
                && _lastTimeSinceStartup > 0
            );

            {
                _refreshIcon.text = importText;
                if (GUILayout.Button(_refreshIcon, GUILayout.Height(25)))
                {
                    _lastTimeSinceStartup = (float) EditorApplication.timeSinceStartup;
                    ImportLanguages();
                }
            }
            {
                _searchIcon.text = " Importer List";
                if (GUILayout.Button(_searchIcon, GUILayout.Height(25)))
                {
                    CuvImportersWidow.ShowWindow();
                }
            }

            EditorGUILayout.EndHorizontal();
            
            {
                EditorGUILayout.Space(5);
                EditorGUILayout.HelpBox("How to get languages\nLanguages are obtained from the generated CuvImporter. If languages are not displayed, it means either there is no CuvImporter or localization-enabled Client is not being used. Please configure CuvImporter and re-import.", MessageType.Info);
            }
            EditorGUILayout.EndVertical();
            
            _serializedObject.Update();
            
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

            {
                var style = new GUIStyle()
                {
                    padding = new RectOffset(10, 10, 0, 10)
                };
                EditorGUILayout.BeginVertical(style);
            }

            {
                var style = new GUIStyle(GUI.skin.box)
                {
                    padding = new RectOffset(10, 10, 10, 10)
                };
                EditorGUILayout.BeginVertical(style);
                EditorGUILayout.LabelField("[Editor Only]");
                EditorGUILayout.HelpBox("This is a language test that can only be executed in the Editor. It will be ignored on actual devices.", MessageType.Info);
                EditorGUILayout.PropertyField(_testLanguageProp);
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(5);
            }
            
            EditorGUILayout.HelpBox("All languages. Select the languages to use by checking the checkboxes.", MessageType.Info);
            EditorGUILayout.PropertyField(_languageStatusProp);
                
            EditorGUILayout.HelpBox("Default language. If the specified language does not exist, this language will be used.", MessageType.Info);
            EditorGUILayout.PropertyField(_defaultLanguageProp);

            EditorGUILayout.HelpBox("Priority display language. Selecting \"" + CuvLanguageSettings.AppLangName + "\" will prioritize the device language.", MessageType.Info);
            EditorGUILayout.PropertyField(_startLanguageProp);

            EditorGUILayout.HelpBox("When language is saved, from the second launch onwards, the selected language will be prioritized ignoring Start Language. (Setting for users who want to use a different language from their OS language)", MessageType.Info);
            EditorGUILayout.PropertyField(_saveLanguageProp);

            EditorGUILayout.EndVertical();
            
            EditorGUILayout.EndScrollView();
            
            EditorGUI.EndDisabledGroup();
            
            _serializedObject.ApplyModifiedProperties();
        }
    }
}