
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CMSuniVortex
{
    /// <summary>
    /// Managing a reference to a list of models.
    /// </summary>
    public abstract class CuvReference<T, TS> : ScriptableObject, ICuvReference<T>, ICuvLocalizedReference 
        where T : ICuvModel 
        where TS : ScriptableObject, ICuvModelList<T>
    {
        public event Action OnInitializeLocalize = delegate {};
        public event Action<SystemLanguage> OnChangeLanguage = delegate {};
        
        [SerializeField] TS[] _modelLists;

        public int ContentsLength => ActiveLocalizedList.Length;
        public bool IsInitializedLocalize { get; private set; }
        public SystemLanguage ActiveLanguage
        {
            get
            {
                if(!_isLocalizationInitializeReady)
                {
                    InitializeLocalization();
                }
                Assert.IsTrue(IsLocalizedData, name + " This reference is not translated data, so Language cannot be retrieved.");
                return _modelLanguages[_currentLanguageIndex];
            }
        }
        public TS ActiveLocalizedList => _modelLists[_currentLanguageIndex];
        public bool IsLocalizedData => _modelLanguages?.Length > 0;
        
        SystemLanguage[] _modelLanguages;
        SystemLanguage _defaultLanguage;
        int _currentLanguageIndex;
        bool _isLocalizationInitializeReady;

        protected virtual void OnEnable()
        {
            if(!_isLocalizationInitializeReady)
            {
                InitializeLocalization();
            }
            
            if (IsRunTtime())
            {
                if (IsLocalizedData)
                {
                    SceneManager.sceneLoaded += OnSceneLoaded;
                }
#if UNITY_EDITOR
                EditorApplication.playModeStateChanged += LogPlayModeState;
#endif
            }
        }

        void OnDisable()
        {
            if (CuvLanguageSwitcher.Exists)
            {
                CuvLanguageSwitcher.Instance.OnChangeLanguage -= OnChangeLanguageInternal;
            }
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
        
        void OnSceneLoaded(Scene prev, LoadSceneMode next)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            var switcher = CuvLanguageSwitcher.Instance;
            switcher.SetDefaultLanguage(_defaultLanguage);
            switcher.AddLanguages(this);
            switcher.OnChangeLanguage += OnChangeLanguageInternal;
            if (switcher.IsInitialized)
            {
                var language = switcher.ActiveLanguage;
                OnChangeLanguageInternal(language);
            }
        }
        
        void InitializeLocalization()
        {
            _isLocalizationInitializeReady = true;
            if (_modelLists is not {Length: > 0}
                || !Enum.IsDefined(typeof(SystemLanguage), _modelLists[0].CuvId))
            {
                return;
            }
            _modelLanguages = new SystemLanguage[_modelLists.Length];
            for (var i = 0; i < _modelLanguages.Length; i++)
            {
                var language = Enum.Parse<SystemLanguage>(_modelLists[i].CuvId);
                _modelLanguages[i] = language;
                if (i == 0)
                {
                    _defaultLanguage = language;
                }
            }
        }
        
#if UNITY_EDITOR
        void LogPlayModeState(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingPlayMode)
            {
                EditorApplication.playModeStateChanged -= LogPlayModeState;
                Resources.UnloadAsset(this);
            }
        }
#endif
        
        public TS GetListFirst() => _modelLists[0];
        
        public TS GetListLatest() => _modelLists[^1];
        
        public TS GetListAt(int index) => _modelLists[index];
        
        public void SetModelLists(TS[] modelLists) => _modelLists = modelLists;
        
        public ICuvModelList<T> GetModelList(string id)
        {
            foreach (var list in _modelLists)
            {
                if (list.CuvId == id)
                {
                    return list;
                }
            }
            throw new ArgumentException($"The model list with CuvId '{id}' does not exist.");
        }
        
        [Obsolete("This method is obsolete. Please use GetByKey instead.")]
        public T GetById(string id) => GetByKey(id);
        
        public T GetByKey(string key)
        {
            var model = ActiveLocalizedList.GetByKey(key);
            Assert.IsTrue(model != null, "Id that does not exist : " + key);
            return model;
        }
        
        [Obsolete("This method is obsolete. Please use TryGetByKey instead.")]
        public bool TryGetById(string id, out T model) => TryGetByKey(id, out model);
        
        public bool TryGetByKey(string key, out T model) => ActiveLocalizedList.TryGetByKey(key, out model);
        
        public IReadOnlyList<SystemLanguage> GetLanguages()
        {
            if (!_isLocalizationInitializeReady)
            {
                InitializeLocalization();
            }
            return _modelLanguages;
        }
        
        public void ChangeLanguage(SystemLanguage language)
        {
            if (!_isLocalizationInitializeReady)
            {
                InitializeLocalization();
            }
            CuvLanguageSwitcher.Instance.ChangeLanguage(language);
        }

        public IEnumerator WaitForLoadLocalizationCo(Action onReady = default)
        {
            yield return new WaitUntil(() => IsInitializedLocalize);
            onReady?.Invoke();
        }
        
        public async Task WaitForLoadLocalizationAsync(CancellationToken token = default)
            => await TaskSupport.WaitUntilAsync(() => IsInitializedLocalize, token);
        
        void OnChangeLanguageInternal(SystemLanguage language)
        {
            for (var i = 0; i < _modelLists.Length; i++)
            {
                if (_modelLanguages[i] == language)
                {
                    SetLanguageAt(i);
                    return;
                }
            }
            SetLanguageAt(0);
        }
        
        void SetLanguageAt(int index)
        {
            if (IsInitializedLocalize
                && _currentLanguageIndex == index)
            {
                return;
            }
            if (!IsInitializedLocalize)
            {
                OnInitializeLocalize();
            }
            IsInitializedLocalize = true;
            _currentLanguageIndex = index;
            OnChangeLanguage(_modelLanguages[index]);
        }

        public bool HasContents()
            => _modelLists is {Length: > 0}
               && _modelLists[0].Length > 0;

        public string[] GetKeys()
        {
            if (_modelLists == default
                || _modelLists.Length == 0)
            {
                return Array.Empty<string>();
            }
            var list = _modelLists[0];
            var keys = new string[list.Length];
            for (var i = 0; i < list.Length; i++)
            {
                keys[i] = list.GetAt(i).GetKey();
            }
            return keys;
        }

        public string[] GetIds()
        {
            if (_modelLists == default
                || _modelLists.Length == 0)
            {
                return Array.Empty<string>();
            }
            var result = new string[_modelLists.Length];
            for (var i = 0; i < _modelLists.Length; i++)
            {
                result[i] = _modelLists[i].CuvId;
            }
            return result;
        }
        
        public static bool IsRunTtime()
        {
#if UNITY_EDITOR
            return EditorApplication.isPlayingOrWillChangePlaymode;
#else
            return true;
#endif
        }
    }
}