
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CMSuniVortex
{
    public abstract class CuvLanguages : MonoBehaviour
    {
        [SerializeField] bool _isLanguageMissingTest;
        
        public SystemLanguage ActiveLanguage => CuvLanguageSwitcher.Instance.ActiveLanguage;

        public IReadOnlyList<SystemLanguage> Languages
        {
            get
            {
                if (_languages == default)
                {
                    Debug.LogWarning("CuvLanguages is not initialized.");
                }
                return _languages;
            }
        }
        
        SystemLanguage[] _languages;

        protected abstract void OnInitialized();
        
        protected virtual IEnumerator Start()
        {
            yield return new WaitForEndOfFrame();
            
            if (CuvLanguageSwitcher.Instance.IsInitialized)
            {
                Initialize();
            }
            else
            {
                CuvLanguageSwitcher.Instance.OnInitialized += Initialize;
            }
        }

        void Initialize()
        {
            CuvLanguageSwitcher.Instance.OnInitialized -= Initialize;
            _languages = CuvLanguageSwitcher.Instance.GetLanguages();
#if UNITY_EDITOR
            if (_isLanguageMissingTest)
            {
                Array.Resize(ref _languages, _languages.Length + 3);
                _languages[^3] = SystemLanguage.Afrikaans;
                _languages[^2] = SystemLanguage.Basque;
                _languages[^1] = SystemLanguage.Latvian;
            }
#endif
            OnInitialized();
        }

        protected void ChangeLanguage(SystemLanguage language)
            => CuvLanguageSwitcher.Instance.ChangeLanguage(language);

        protected SystemLanguage GetLanguageAt(int index)
        {
            for (var i = 0; i < _languages.Length; i++)
            {
                if (i == index)
                {
                    return _languages[i];
                }
            }
            throw new ArgumentException("Language not found. index: " + index + ".");
        }
        
        protected int GetLanguageIndex(SystemLanguage language)
        {
            for (var i = 0; i < _languages.Length; i++)
            {
                if (_languages[i] == language)
                {
                    return i;
                }
            }
            throw new ArgumentException( "Language not found. language: " + language + ".");
        }
    }
}