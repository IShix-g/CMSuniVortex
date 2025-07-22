#if ENABLE_ADDRESSABLES
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.Exceptions;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CMSuniVortex
{
    [DefaultExecutionOrder(-5000)]
    public abstract class CuvAddressableList<T, TS, TR> : MonoBehaviour
        where T : ICuvModel
        where TS : Object, ICuvModelList<T>
        where TR : ScriptableObject, ICuvAsyncReference<T, TS>
    {
        [SerializeField] TR _reference;
        [SerializeField, CuvListId("_reference")] string _id;
        [SerializeField] bool _autoLoading = true;
        
        public string Id
        {
            get
            {
                Assert.IsFalse(string.IsNullOrEmpty(_id), "Please set the Id from Inspector. and call ");
                return _id;
            }
        }
        public TS ActiveList
        {
            get
            {
                if(_handle.IsValid()
                   && _handle.IsDone)
                {
                    return _handle.Result;
                }
                throw new OperationException("Please access after initialization is complete. OnInitialized is the fastest way.");
            }
        }
        public bool IsLoading => _handle.IsValid() && !_handle.IsDone;
        public bool IsLoaded => _handle.IsValid() && _handle.IsDone;
        public AssetReferenceT<TS> ListAssetReference
        {
            get
            {
                Assert.IsFalse(string.IsNullOrEmpty(_id), "Please set the Id from Inspector.");
                return _reference.FindModelListById(_id);
            }
        }
        
        AsyncOperationHandle<TS> _handle;

        protected virtual void Awake()
        {
            if (_autoLoading)
            {
                Initialize();
            }
        }

        protected virtual void OnDestroy() => Release();

        protected abstract void OnLoaded();
        
        public void Initialize() => LoadModelListAsync().Handled();
        
        async Task<TS> LoadModelListAsync()
        {
            if (_handle.IsValid()
                && _handle.IsDone)
            {
                if (_handle.Result.CuvId == Id)
                {
                    return _handle.Result;
                }
            
                Release();
            }
            
            _handle = Addressables.LoadAssetAsync<TS>(ListAssetReference);
            await _handle.Task;
            
            switch (_handle.Status)
            {
                case AsyncOperationStatus.Succeeded:
                    OnLoaded();
                    return _handle.Result;
                case AsyncOperationStatus.Failed:
                    throw _handle.OperationException;
            }
            return default;
        }
        
        public void Release()
        {
            if (_handle.IsValid())
            {
                Addressables.Release(_handle);
            }
        }
        
        protected virtual void Reset()
        {
#if UNITY_EDITOR
            _reference = AssetDatabase.FindAssets("t:" + typeof(ScriptableObject))
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<ScriptableObject>)
                .FirstOrDefault(x => x is TR) as TR;

            if (_reference != default)
            {
                var ids = _reference.GetIds();
                _id = ids.Length > 0 ? ids[0] : string.Empty;
            }
#endif
        }
    }
}
#endif