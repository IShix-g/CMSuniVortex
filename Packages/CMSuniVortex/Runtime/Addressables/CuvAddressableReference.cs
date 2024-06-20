#if ENABLE_ADDRESSABLES
using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CMSuniVortex.Addressable
{
    /// <summary>
    /// Managing a reference to a list of models.
    /// </summary>
    public abstract class CuvAddressableReference<T, TS> : ScriptableObject, ICuvAsyncReference where T : ICuvModel where TS : Object, ICuvModelList<T>
    {
        public event Action<SystemLanguage> OnChangeLanguage = delegate {};
        
        [SerializeField] AddressableModel<T, TS>[] _modelLists;

        public bool IsInitialized { get; private set; }
        public bool IsLoading { get; private set; }
        public SystemLanguage Language { get; private set; }
        public TS Current { get; private set; }
        
        [NonSerialized] AsyncOperationHandle<TS> _handle;

        public void SetModelLists(AddressableModel<T, TS>[] modelLists) => _modelLists = modelLists;
        
        public IEnumerator Initialize(Action onLoaded = default)
        {
            yield return Initialize(Application.systemLanguage, onLoaded);
        }

        public IEnumerator Initialize(SystemLanguage language, Action onLoaded = default)
        {
            if (!IsInitialized)
            {
                IsInitialized = true;
                yield return ChangeLanguage(language, onLoaded);
            }
            else
            {
                onLoaded?.Invoke();
            }
        }
        
        public async Task InitializeAsync()
             => await InitializeAsync(Application.systemLanguage);
        
        public async Task InitializeAsync(SystemLanguage language)
        {
            if (!IsInitialized)
            {
                IsInitialized = true;
                await ChangeLanguageAsync(language);
            }
        }

        public TS GetList() => Current;

        public T GetById(string id)
        {
            if (GetList().TryGetByKey(id, out var model))
            {
                return model;
            }
            throw new ArgumentException("Id that does not exist : " + id);
        }
        
        public bool TryGetById(string id, out T model)
        {
            model = GetList().GetByKey(id);
            return model != null;
        }

        public T GetByIndex(int index)
            => GetList().GetByIndex(index);
        
        public IEnumerator ChangeLanguage(SystemLanguage language, Action onLoaded)
        {
            foreach (var list in _modelLists)
            {
                if (list.Language == language)
                {
                    yield return SetLanguage(list);
                    onLoaded?.Invoke();
                    yield break;
                }
            }
            yield return SetLanguage(_modelLists[0]);
            onLoaded?.Invoke();
        }
        
        public async Task ChangeLanguageAsync(SystemLanguage language)
        {
            foreach (var list in _modelLists)
            {
                if (list.Language == language)
                {
                    await SetLanguageAsync(list);
                    return;
                }
            }
            await SetLanguageAsync(_modelLists[0]);
        }

        public bool HasContents() => Current is {Length: > 0};
        
        public string[] GetKeys()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                if (Current == default
                    || Current.Length == 0)
                {
                    if (_modelLists is {Length: > 0}
                        && _modelLists[0].List != default)
                    {
                        var path = AssetDatabase.GUIDToAssetPath(_modelLists[0].List.AssetGUID);
                        var obj = AssetDatabase.LoadAssetAtPath<TS>(path);
                        Current = obj;
                    }
                }
            }
#endif
            if (Current == default
                || Current.Length == 0)
            {
                return Array.Empty<string>();
            }
            var keys = new string[Current.Length];
            for (var i = 0; i < Current.Length; i++)
            {
                keys[i] = Current.GetByIndex(i).GetKey();
            }
            return keys;
        }

        public void Release()
        {
            if (_handle.IsValid()
                && _handle.IsDone)
            {
                Current = default;
                Addressables.Release(_handle);
            }
        }
        
        IEnumerator SetLanguage(AddressableModel<T, TS> newModel)
        {
            if (Language == newModel.Language)
            {
                yield break;
            }
            Language = newModel.Language;

            if (_handle.IsValid()
                && _handle.IsDone)
            {
                Current = default;
                Addressables.Release(_handle);
            }

            IsLoading = true;
            _handle = newModel.List.LoadAssetAsync();
            yield return _handle;

            switch (_handle.Status)
            {
                case AsyncOperationStatus.Succeeded:
                    Current = _handle.Result;
                    OnChangeLanguage(_handle.Result.Language);
                    break;
                case AsyncOperationStatus.Failed:
                    Debug.LogError($"Language could not be loaded. Reason: {_handle.OperationException}");
                    break;
            }
            IsLoading = false;
        }
        
        async Task SetLanguageAsync(AddressableModel<T, TS> newModel)
        {
            if (Language == newModel.Language)
            {
                return;
            }
    
            Language = newModel.Language;

            if (_handle.IsValid()
                && _handle.IsDone)
            {
                Current = default;
                Addressables.Release(_handle);
            }

            IsLoading = true;
            _handle = newModel.List.LoadAssetAsync();
            await _handle.Task;

            switch (_handle.Status)
            {
                case AsyncOperationStatus.Succeeded:
                    Current = _handle.Result;
                    OnChangeLanguage(_handle.Result.Language);
                    break;
                case AsyncOperationStatus.Failed:
                    Debug.LogError($"Language could not be loaded. Reason: {_handle.OperationException}");
                    break;
            }
            IsLoading = false;
        }
    }
}
#endif