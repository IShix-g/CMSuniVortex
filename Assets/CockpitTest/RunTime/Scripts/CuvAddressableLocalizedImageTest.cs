
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace Test.Cockpit
{
    [RequireComponent(typeof(Image))]
    public sealed class CuvAddressableLocalizedImageTest : CuvAddressableLocalizedTest
    {
        [SerializeField] Image _image;

        AsyncOperationHandle<Sprite> _handle;
        
        protected override async void OnChangeLanguage(CatAddressableDetailsLocalize model)
        {
            _handle = Addressables.LoadAssetAsync<Sprite>(model.Image);
            await _handle.Task;
            if (_handle.Status == AsyncOperationStatus.Succeeded)
            {
                _image.sprite = _handle.Result;
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
        }

        protected override void Reset()
        {
            base.Reset();
            _image = GetComponent<Image>();
        }
    }
}