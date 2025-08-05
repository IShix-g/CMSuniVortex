
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using CMSuniVortex;

namespace Test.Excel
{
    public sealed class InitializeAddressableLocalizationTest : MonoBehaviour
    {
        [SerializeField] EModelLocalizedAddressableExcelCuvAddressableReference _reference;
        [SerializeField] SpriteRenderer _spriteRenderer;
        
        CancellationTokenSource _cts;
        AsyncOperationHandle<Sprite> _handle;
        
        async void Start()
        {
            Debug.Log("Start Initialize Addressables flag: " + _reference.IsInitializedLocalize);
            if (!_reference.IsInitializedLocalize)
            {
                _cts = new CancellationTokenSource();
                await _reference.WaitForLoadLocalizationAsync(_cts.Token);
            }
            Debug.Log("End Initialize Addressables flag: " + _reference.IsInitializedLocalize);
        }

        void OnEnable()
        {
            _reference.OnLoadedLanguage += OnLoadedLanguage;
            if (_reference.IsInitializedLocalize
                && !_reference.IsLoading)
            {
                OnLoadedLanguage(_reference.ActiveLanguage);
            }
        }

        void OnDisable()
        {
            _reference.OnLoadedLanguage -= OnLoadedLanguage;
        }
        
        async void OnLoadedLanguage(SystemLanguage language)
        {
            var obj = _reference.ActiveLocalizedList[2];
            Debug.Log("IsLoaded Addressables : " + obj.Text);
            
            _handle = Addressables.LoadAssetAsync<Sprite>(obj.Image);
            await _handle.Task;
            if (_handle.Status == AsyncOperationStatus.Succeeded)
            {
                _spriteRenderer.sprite = _handle.Result;
            }
            else if (_handle.Status == AsyncOperationStatus.Failed)
            {
                Debug.LogError(_handle.OperationException);
            }
        }
        
        void OnDestroy()
        {
            if (_handle.IsValid())
            {
                Addressables.Release(_handle);
            }
            _cts?.SafeCancelAndDispose();
        }
    }
}