
using System;
using System.Threading;
using UnityEngine;
using CMSuniVortex;
using UnityEngine.UI;

namespace Test.Excel
{
    public class InitializeLocalizationCallInitializeTest : MonoBehaviour
    {
        [SerializeField] EModelCallInitializeExcelCuvReference _reference;
        [SerializeField] SpriteRenderer _spriteRenderer;
        [SerializeField] Button _initializeButton;
        
        CancellationTokenSource _cts;

        async void Start()
        {
            _initializeButton.onClick.AddListener(ClickButton);
            
            Debug.Log("Start Initialize flag: " + _reference.IsInitializedLocalize);

            try
            {
                if (!_reference.IsInitializedLocalize)
                {
                    _cts = new CancellationTokenSource();
                    await _reference.WaitForLoadLocalizationAsync(_cts.Token);
                }
            }
            catch (OperationCanceledException)
            {
                Debug.LogWarning("Operation canceled");
                return;
            }
            
            Debug.Log("End Initialize flag: " + _reference.IsInitializedLocalize);
        }

        void OnEnable()
        {
            _reference.OnChangeLanguage += OnLoadedLanguage;
            if (_reference.IsInitializedLocalize)
            {
                OnLoadedLanguage(_reference.ActiveLanguage);
            }
        }

        void OnDisable()
        {
            _reference.OnChangeLanguage -= OnLoadedLanguage;
        }

        void ClickButton()
        {
            if (!_reference.IsInitializedLocalize)
            {
                _reference.InitializeLocalize();
            }
        }
        
        void OnLoadedLanguage(SystemLanguage language)
        {
            var obj = _reference.ActiveLocalizedList[0];
            Debug.Log("IsLoaded : " + obj.Text);
            _spriteRenderer.sprite = obj.Image;
        }

        void OnDestroy()
        {
            _cts?.SafeCancelAndDispose();
        }
    }
}
