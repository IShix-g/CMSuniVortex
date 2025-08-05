
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using CMSuniVortex;

namespace Test.Excel
{
    public sealed class InitializeLocalizationTest : MonoBehaviour
    {
        [SerializeField] EModelLocalizedExcelCuvReference _reference;

        CancellationTokenSource _cts;
        AsyncOperationHandle<Sprite> _handle;
        
        async void Start()
        {
            Debug.Log("Start Initialize flag: " + _reference.IsInitializedLocalize);
            if (!_reference.IsInitializedLocalize)
            {
                _cts = new CancellationTokenSource();
                await _reference.WaitForLoadLocalizationAsync(_cts.Token);
            }
            Debug.Log("End Initialize flag: " + _reference.IsInitializedLocalize);
        }
        
        void OnEnable()
        {
            _reference.OnChangeLanguage += OnChangeLanguage;
            if (_reference.IsInitializedLocalize)
            {
                OnChangeLanguage(_reference.ActiveLanguage);
            }
        }

        void OnDisable()
        {
            _reference.OnChangeLanguage -= OnChangeLanguage;
        }
        
        void OnChangeLanguage(SystemLanguage language)
        {
            var obj = _reference.ActiveLocalizedList[1];
            Debug.Log("IsLoaded : " + obj.Text);
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