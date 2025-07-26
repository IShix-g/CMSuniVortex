#if ENABLE_ADDRESSABLES
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CMSuniVortex.Addressable
{
    /// <summary>
    /// Managing a reference to a list of models.
    /// </summary>
    public abstract class CuvAddressableReference<T, TS> : ScriptableObject, ICuvAsyncReference<T, TS>, ICuvLocalizedAsyncReference 
        where T : ICuvModel 
        where TS : Object, ICuvModelList<T>
    {
        public event Action OnInitializeLocalize = delegate {};
        public event Action<SystemLanguage> OnStartLoadLanguage = delegate {};
        public event Action<SystemLanguage> OnLoadedLanguage = delegate {};
        
        [SerializeField] AddressableModel<T, TS>[] _modelLists;

        public int ContentsLength => ActiveLocalizedList != default ? ActiveLocalizedList.Length : 0;
        public bool IsInitializedLocalize => _isInitialized && ActiveLocalizedList != default;
        public bool IsLoading { get; private set; }

        public SystemLanguage ActiveLanguage
        {
            get
            {
                if(!_isLocalizationInitializeReady)
                {
                    PrepareLocalization();
                }
                Assert.IsTrue(IsLocalizedData, name + " This reference is not translated data, so Language cannot be retrieved.");
                return _modelLanguages[_currentLanguageIndex];
            }
        }
        /// <summary>
        /// Current translation data loaded from Addressables.
        /// </summary>
        public TS ActiveLocalizedList { get; private set; }
        public bool IsLocalizedData => _modelLanguages is {Length: > 0};
        public virtual bool EnableAutoLocalization => true;
        
        SystemLanguage[] _modelLanguages;
        SystemLanguage _defaultLanguage;
        AsyncOperationHandle<TS> _localizationHandle;
        bool _isLocalizationInitializeReady;
        int _currentLanguageIndex;
        bool _isInitialized;
        
        protected virtual void OnEnable()
        {
            if (!_isLocalizationInitializeReady)
            {
                PrepareLocalization();
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
            if (!IsRunTtime())
            {
                return;
            }
            
            if (_localizationHandle.IsValid())
            {
                Release();
            }
            if (IsLocalizedData)
            {
                if (CuvLanguageSwitcher.Exists)
                {
                    CuvLanguageSwitcher.Instance.OnChangeLanguage -= OnChangeLanguageInternal;
                }
                SceneManager.sceneLoaded -= OnSceneLoaded;
            }
        }
        
        void PrepareLocalization()
        {
            _isLocalizationInitializeReady = true;
            if (_modelLists is not {Length: > 0}
                || _modelLists[0] == default)
            {
                return;
            }
            var firstKey = _modelLists[0].CuvId;
            if (firstKey == null ||
                !Enum.IsDefined(typeof(SystemLanguage), firstKey))
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

        void OnSceneLoaded(Scene prev, LoadSceneMode next)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            var switcher = CuvLanguageSwitcher.Instance;
            if (switcher.HasDefaultLanguage
                && TryFindLanguage(switcher.DefaultLanguage, out var defaultLanguage))
            {
                _defaultLanguage = defaultLanguage;
            }
            else
            {
                switcher.SetDefaultLanguage(_defaultLanguage);
            }
            switcher.AddLanguages(this);
            
            if (EnableAutoLocalization)
            {
                InitializeLocalizeInternal();
            }
        }
        
#if UNITY_EDITOR
        void LogPlayModeState(PlayModeStateChange state)
        {
            if (state != PlayModeStateChange.ExitingPlayMode)
            {
                return;
            }
            EditorApplication.playModeStateChanged -= LogPlayModeState;
            Resources.UnloadAsset(this);
        }
#endif

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
        
        public AddressableModel<T, TS> GetListFirst() => _modelLists[0];
        public AddressableModel<T, TS> GetListLatest() => _modelLists[^1];
        public AddressableModel<T, TS> GetListAt(int index) => _modelLists[index];
        
        public void SetModelLists(AddressableModel<T, TS>[] modelLists) => _modelLists = modelLists;

        public AssetReferenceT<TS> FindModelListById(string id)
            => FindModelListByIdInternal(id).List;
        
        AddressableModel<T, TS> FindModelListByIdInternal(string id)
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
        
        public IEnumerator LoadModelListCo(string id, Action<AsyncOperationHandle<TS>> onLoaded, Action<string> onError = default)
        {
            var model = FindModelListByIdInternal(id);
            yield return LoadModelListCo(model, onLoaded, onError);
        }

        public IEnumerator LoadModelListCo(AddressableModel<T, TS> model, Action<AsyncOperationHandle<TS>> onLoaded, Action<string> onError = default)
        {
            IsLoading = true;
            try
            {
                var handle = Addressables.LoadAssetAsync<TS>(model.List);
                yield return handle;

                switch (handle.Status)
                {
                    case AsyncOperationStatus.Succeeded:
                        onLoaded?.Invoke(handle);
                        break;
                    case AsyncOperationStatus.Failed:
                        var msg = $"Language could not be loaded. Reason: {handle.OperationException}";
                        onError?.Invoke(msg);
                        Debug.LogError(msg);
                        break;
                    case AsyncOperationStatus.None:
                        throw new InvalidOperationException("Id: " + model.CuvId + " Operation status None encountered.");
                    default:
                        throw new InvalidOperationException("Id: " + model.CuvId + " Unexpected AsyncOperationStatus.");
                }
            }
            finally
            {
                IsLoading = false;
            }
        }
        
        public async Task<AsyncOperationHandle<TS>> LoadModelListAsync(string id)
        {
            var model = FindModelListByIdInternal(id);
            return await LoadModelListAsync(model);
        }
        
        async Task<AsyncOperationHandle<TS>> LoadModelListAsync(AddressableModel<T, TS> model, CancellationToken token = default)
        {
            IsLoading = true;
            try
            {
                var handle = Addressables.LoadAssetAsync<TS>(model.List);
                await handle.Task;

                switch (handle.Status)
                {
                    case AsyncOperationStatus.Succeeded:
                        return handle;
                    case AsyncOperationStatus.Failed:
                        throw handle.OperationException;
                    case AsyncOperationStatus.None:
                        throw new InvalidOperationException("Id: " + model.CuvId + " Operation status None encountered.");
                    default:
                        throw new InvalidOperationException("Id: " + model.CuvId + " Unexpected AsyncOperationStatus.");
                }
            }
            finally
            {
                IsLoading = false;
            }
        }

        public void InitializeLocalize()
        {
            if (EnableAutoLocalization)
            {
                Debug.LogWarning("Auto initialization is enabled, so manual initialization is not possible. If needed, override \"" + nameof(EnableAutoLocalization) + "\".");
            }
            else if (!IsLoading
                     && !IsInitializedLocalize)
            {
                InitializeLocalizeInternal();
            }
        }

        public IEnumerator InitializeLocalizeCo(Action onReady = default)
        {
            if (!IsLoading
                && !IsInitializedLocalize)
            {
                InitializeLocalize();
                yield return WaitForLoadLocalizationCo(onReady);
            }
        }
        
        public async Task InitializeLocalizeAsync(CancellationToken token = default)
        {
            if (!IsLoading
                && !IsInitializedLocalize)
            {
                InitializeLocalize();
                await WaitForLoadLocalizationAsync(token);
            }
        }

        void InitializeLocalizeInternal()
        {
            var switcher = CuvLanguageSwitcher.Instance;
            switcher.OnChangeLanguage += OnChangeLanguageInternal;
            if (switcher.IsInitialized)
            {
                var language = switcher.ActiveLanguage;
                OnChangeLanguageInternal(language);
            }
        }
        
        public SystemLanguage FindLanguage() => FindLanguage(Application.systemLanguage);

        public SystemLanguage FindLanguage(SystemLanguage current)
        {
            if(!_isLocalizationInitializeReady)
            {
                PrepareLocalization();
            }
            foreach (var lang in _modelLanguages)
            {
                if (lang == current)
                {
                    return lang;
                }
            }
            return _defaultLanguage;
        }

        public bool TryFindLanguage(SystemLanguage current, out SystemLanguage language)
        {
            if(!_isLocalizationInitializeReady)
            {
                PrepareLocalization();
            }
            foreach (var lang in _modelLanguages)
            {
                if (lang == current)
                {
                    language = lang;
                    return true;
                }
            }
            language = _defaultLanguage;
            return false;
        }

        public AddressableModel<T, TS> FindModelList(SystemLanguage current)
        {
            for (var i = 0; i < _modelLanguages.Length; i++)
            {
                if (_modelLanguages[i] == current)
                {
                    return _modelLists[i];
                }
            }
            return _modelLists[0];
        }
        
        public void ChangeLanguage(SystemLanguage language) 
            => CuvLanguageSwitcher.Instance.ChangeLanguage(language);
        
        public bool HasContents() => ActiveLocalizedList is {Length: > 0};

        public string[] GetKeys()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                if (ActiveLocalizedList == default
                    || ActiveLocalizedList.Length == 0)
                {
                    if (_modelLists is {Length: > 0}
                        && _modelLists[0].List != default)
                    {
                        var path = AssetDatabase.GUIDToAssetPath(_modelLists[0].List.AssetGUID);
                        var obj = AssetDatabase.LoadAssetAtPath<TS>(path);
                        ActiveLocalizedList = obj;
                    }
                }
            }
#endif
            if (ActiveLocalizedList == default
                || ActiveLocalizedList.Length == 0)
            {
                return Array.Empty<string>();
            }
            var keys = new string[ActiveLocalizedList.Length];
            for (var i = 0; i < ActiveLocalizedList.Length; i++)
            {
                keys[i] = ActiveLocalizedList.GetAt(i).GetKey();
            }
            return keys;
        }

        public void Cancel()
        {
            if (_localizationHandle.IsValid()
                && !_localizationHandle.IsDone)
            {
                Addressables.Release(_localizationHandle);
            }
        }
        
        public void Release()
        {
            if (_localizationHandle.IsValid())
            {
                ActiveLocalizedList = default;
                Addressables.Release(_localizationHandle);
            }
        }
        
        public IEnumerator WaitForLoadLocalizationCo(Action onReady = default)
        {
            yield return new WaitUntil(() => !IsLoading && IsInitializedLocalize);
            onReady?.Invoke();
        }
        
        public async Task WaitForLoadLocalizationAsync(CancellationToken token = default)
            => await TaskSupport.WaitUntilAsync(() => !IsLoading && IsInitializedLocalize, token);

        void OnChangeLanguageInternal(SystemLanguage language)
            => ChangeLanguageAsync(language).Handled();
        
        async Task ChangeLanguageAsync(SystemLanguage language)
        {
            language = FindLanguage(language);
            var modelList = FindModelList(language);
            await SetLanguageAsync(modelList);
        }
        
        async Task SetLanguageAsync(AddressableModel<T, TS> newModel)
        {
            IsLoading = true;
            var modelIndex = FindModelIndex(newModel);
            if (IsInitializedLocalize
                && ActiveLanguage == _modelLanguages[modelIndex])
            {
                IsLoading = false;
                return;
            }

            try
            {
                _currentLanguageIndex = modelIndex;
                OnStartLoadLanguage(_modelLanguages[modelIndex]);

                if (_localizationHandle.IsValid())
                {
                    ActiveLocalizedList = default;
                    Addressables.Release(_localizationHandle);
                }
            
                _localizationHandle = Addressables.LoadAssetAsync<TS>(newModel.List);
                await _localizationHandle.Task;

                switch (_localizationHandle.Status)
                {
                    case AsyncOperationStatus.Succeeded:
                        ActiveLocalizedList = _localizationHandle.Result;
                        if (Enum.TryParse<SystemLanguage>(_localizationHandle.Result.CuvId, out var language))
                        {
                            OnLoadedLanguage(language);
                        }
                        break;
                    case AsyncOperationStatus.Failed:
                        Debug.LogError($"Language could not be loaded. Reason: {_localizationHandle.OperationException}");
                        break;
                    case AsyncOperationStatus.None:
                        throw new InvalidOperationException("Id: " + newModel.CuvId + " Operation status None encountered.");
                    default:
                        throw new InvalidOperationException("Id: " + newModel.CuvId + " Unexpected AsyncOperationStatus.");
                }
            
                if (!_isInitialized)
                {
                    _isInitialized = true;
                    OnInitializeLocalize();
                }
            }
            finally
            {
                IsLoading = false;
            }
        }
        
        int FindModelIndex(AddressableModel<T, TS> newModel)
        {
            for (var i = 0; i < _modelLists.Length; i++)
            {
                if (_modelLists[i].CuvId == newModel.CuvId)
                {
                    return i;
                }
            }
            return -1;
        }

        public string[] GetIds()
        {
            var result = new string[_modelLists.Length];
            for (var i = 0; i < _modelLists.Length; i++)
            {
                result[i] = _modelLists[i].CuvId;
            }
            return result;
        }

        public IReadOnlyList<SystemLanguage> GetLanguages()
        {
            if(!_isLocalizationInitializeReady)
            {
                PrepareLocalization();
            }
            return _modelLanguages;
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
#endif