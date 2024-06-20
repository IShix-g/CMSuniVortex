
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace Tests
{
    [RequireComponent(typeof(Image))]
    public sealed class TestAddressableImage : TestCatDetailsCockpitCuvAddressableComponent
    {
        [SerializeField] Image _image;

        AsyncOperationHandle<Sprite> _handle;
        
        protected override async void OnChangeLanguage(CatAddressableDetails model)
        {
            _handle = model.Image.LoadAssetAsync();
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

        void OnDestroy() => Addressables.Release(_handle);

        protected override void Reset()
        {
            base.Reset();
            _image = GetComponent<Image>();
        }
    }
}