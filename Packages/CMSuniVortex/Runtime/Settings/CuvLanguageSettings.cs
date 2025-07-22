
using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CMSuniVortex
{
    [CuvScriptableObjectPath("Assets/Resources/CuvLanguageSettings.asset")]
    public sealed class CuvLanguageSettings : CuvScriptableSingleton<CuvLanguageSettings>
    {
        public const string AppLangName = "Application.systemLanguage";

        [SerializeField, CuvAllLanguage] string _languageTest;
        [SerializeField] List<CuvLanguageState> _languages;
        [SerializeField, CuvAllLanguage] string _defaultLanguage;
        [SerializeField, CuvAllLanguage(true)] string _startLanguage = AppLangName;
        [SerializeField] bool _saveLanguage = true;

        public IReadOnlyList<SystemLanguage> Languages
        {
            get
            {
                if (_languageList != default)
                {
                    return _languageList;
                }
                
                _languageList = new List<SystemLanguage>();
                foreach (var language in _languages)
                {
                    if (language.IsActive)
                    {
                        _languageList.Add(language.Language);
                    }
                }
                return _languageList;
            }
        }
        
        public bool HasLanguages => _languages is {Count: > 0};
        public string LanguageTest => _languageTest;
        public string DefaultLanguage => _defaultLanguage;
        public string StartLanguage => _startLanguage;
        public bool SaveLanguage => _saveLanguage;

        [NonSerialized] List<SystemLanguage> _languageList;

#if UNITY_EDITOR
        void OnEnable()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            }
        }

        void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state != PlayModeStateChange.ExitingPlayMode)
            {
                return;
            }
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            Resources.UnloadAsset(this);
        }
#endif  
        
        public void SetDefaultLanguage(SystemLanguage language)
        {
#if UNITY_EDITOR
            _defaultLanguage = language.ToString();
            SaveAsset();
#endif
        }

        public void AddLanguages(IReadOnlyCollection<SystemLanguage> languages)
        {
            _languages ??= new List<CuvLanguageState>();
            foreach (var language in languages)
            {
                if (_languages.Exists(obj => obj.Language == language))
                {
                    continue;
                }
                var obj = new CuvLanguageState(true, language);
                _languages.Add(obj);
            }
            SaveAsset();
        }
        
        public void SaveAsset()
        {
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssetIfDirty(this);
#endif
        }
    }
}