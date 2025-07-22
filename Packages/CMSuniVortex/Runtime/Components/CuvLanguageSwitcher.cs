
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace CMSuniVortex
{
    public sealed class CuvLanguageSwitcher : MonoBehaviour
    {
        const string _savedLanguageKey = "CuvLanguageSwitcher_CuvLanguageSwitcher";
        
        public event Action OnInitialized = delegate {};
        public event Action<SystemLanguage> OnChangeLanguage = delegate {};
        
        public static CuvLanguageSwitcher Instance
        {
            get
            {
                if (s_instance != default)
                {
                    return s_instance;
                }
                s_instance = Create();
                return s_instance;
            }
        }
        static CuvLanguageSwitcher s_instance;
        
        public static bool Exists => s_instance != default;

        public IReadOnlyCollection<SystemLanguage> Languages
        {
            get
            {
                Assert.IsTrue(IsInitialized, "Please access Languages after OnInitialized.");
                return _languages;
            }
        }
        public SystemLanguage ActiveLanguage
        {
            get
            {
                Assert.IsTrue(IsInitialized, "Please access ActiveLanguage after OnInitialized.");
                return _activeLanguage;
            }
        }
        public SystemLanguage DefaultLanguage
        {
            get
            {
                if (_defaultLanguage == default)
                {
                    Debug.LogWarning("Default language is not set. Please reference a ScriptableObject using CuvReference or CuvAddressableReference with Localize from the Inspector.");
                }
                return _defaultLanguage ?? Application.systemLanguage;
            }
        }
        public bool HasDefaultLanguage => _defaultLanguage != default;
        public static string SavedLanguage
        {
            get => PlayerPrefs.GetString(_savedLanguageKey, string.Empty);
            set => PlayerPrefs.SetString(_savedLanguageKey, value);
        }
        public bool IsInitialized { get; private set; }
        
        public CuvLanguageSettings LanguageSettings { get; set; }
        
        readonly HashSet<SystemLanguage> _languages = new ();
        SystemLanguage? _defaultLanguage;
        SystemLanguage _activeLanguage;
        bool _isStarted;

        static CuvLanguageSwitcher Create()
        {
            var go = new GameObject("CuvLanguageSwitcher").AddComponent<CuvLanguageSwitcher>();
            go.LanguageSettings = CuvLanguageSettings.Instance;
            if (go.LanguageSettings != default)
            {
                if (!string.IsNullOrEmpty(go.LanguageSettings.DefaultLanguage)
                    && Enum.TryParse<SystemLanguage>(go.LanguageSettings.DefaultLanguage, out var defaultLanguage))
                {
                    go._defaultLanguage = defaultLanguage;
                }
            }
            if (go.LanguageSettings != default
                && !go.LanguageSettings.SaveLanguage
                && PlayerPrefs.HasKey(_savedLanguageKey))
            {
                PlayerPrefs.DeleteKey(_savedLanguageKey);
            }
            if (go.LanguageSettings != default
                && go.LanguageSettings.HasLanguages)
            {
                var languages = go.LanguageSettings.Languages;
                go._languages.UnionWith(languages);
            }
            go._activeLanguage = Application.systemLanguage;
            DontDestroyOnLoad(go.gameObject);
            return go;
        }

        void Start()
        {
            if (LanguageSettings != default)
            {
#if UNITY_EDITOR
                if (!string.IsNullOrEmpty(LanguageSettings.LanguageTest)
                    && Enum.TryParse<SystemLanguage>(LanguageSettings.LanguageTest, out var testLanguage))
                {
                    _activeLanguage = testLanguage;
                }
                else
#endif
                if (LanguageSettings.SaveLanguage
                    && !string.IsNullOrEmpty(SavedLanguage)
                    && Enum.TryParse<SystemLanguage>(SavedLanguage, out var activeLanguage)
                    && _languages.Contains(activeLanguage))
                {
                    _activeLanguage = activeLanguage;
                }
                else if (!string.IsNullOrEmpty(LanguageSettings.StartLanguage)
                         && LanguageSettings.StartLanguage != CuvLanguageSettings.AppLangName
                         && Enum.TryParse<SystemLanguage>(LanguageSettings.StartLanguage, out var startLanguage)
                         && _languages.Contains(startLanguage))
                {
                    _activeLanguage = startLanguage;
                }
            }

            Initialize(_activeLanguage);
            _isStarted = true;
        }

        public SystemLanguage[] GetLanguages()
        {
            var result = new SystemLanguage[_languages.Count];
            var i = 0;
            foreach (var language in _languages)
            {
                result[i++] = language;
            }
            return result;
        }
        
        public string[] GetLanguagesAsStrings()
        {
            var result = new string[_languages.Count];
            var i = 0;
            foreach (var language in _languages)
            {
                result[i++] = language.ToString();
            }
            return result;
        }
        
        public void SetDefaultLanguage(SystemLanguage language)
        {
            if (_defaultLanguage == default)
            {
                _defaultLanguage = language;
            }
        }
        
        public void AddLanguages(ICuvLanguageSelectable selectable)
        {
            var languages = selectable.GetLanguages();
            AddLanguages(languages);
        }
        
        public void AddLanguages(IEnumerable<SystemLanguage> languages)
        {
            if (LanguageSettings == default
                || !LanguageSettings.HasLanguages)
            {
                _languages.UnionWith(languages);
            }
            if(_isStarted && !IsInitialized)
            {
                Initialize(_activeLanguage);
            }
        }
        
        void Initialize(SystemLanguage language)
        {
            Assert.IsTrue(_languages.Count > 0, "Languages is not set. Please reference a ScriptableObject using CuvReference or CuvAddressableReference with Localize from the Inspector.");
            IsInitialized = true;
            
            if (_defaultLanguage == default)
            {
                if (LanguageSettings != default
                    && !string.IsNullOrEmpty(LanguageSettings.DefaultLanguage)
                    && Enum.TryParse<SystemLanguage>(LanguageSettings.DefaultLanguage, out var defaultLanguage)
                    && _languages.Contains(defaultLanguage))
                {
                    _defaultLanguage = defaultLanguage;
                }
#if DEBUG
                if (_defaultLanguage == default)
                {
                    Debug.LogWarning("Default Language is not set.");
                }
#endif
            }
            else if (!_languages.Contains(_defaultLanguage.Value))
            {
                foreach (var lang in _languages)
                {
                    _defaultLanguage = lang;
                    Debug.LogWarning("Default Language が見つからなかったので言語の1番目の言語を使いました。 Default: " + _defaultLanguage.Value + " -> " + lang);
                    break;
                }
            }
            _activeLanguage = ResolveLanguage(language);
            OnInitialized();
            OnChangeLanguage(_activeLanguage);
        }

        public void ChangeLanguage(SystemLanguage language)
        {
            Assert.IsTrue(IsInitialized, "Please access ChangeLanguage after OnInitialized.");
            language = ResolveLanguage(language);
            
            if(LanguageSettings != default
               && LanguageSettings.SaveLanguage)
            {
                SavedLanguage = language.ToString();
            }
            if (_activeLanguage == language)
            {
                return;
            }
            _activeLanguage = language;
            OnChangeLanguage(language);
        }

        SystemLanguage ResolveLanguage(SystemLanguage language)
        {
            if (_languages.Contains(language))
            {
                return language;
            }
            if (_defaultLanguage != default)
            {
                return _defaultLanguage.Value;
            }
            return Application.systemLanguage;
        }
    }
}