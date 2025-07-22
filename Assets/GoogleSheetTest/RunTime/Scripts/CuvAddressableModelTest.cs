
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using CMSuniVortex;

namespace Test.GoogleSheet
{
    public sealed class CuvAddressableModelTest : CuvAddressableModel<MetaAddressable, MetaAddressableCustomGoogleSheetCuvModelList, MetaAddressableCustomGoogleSheetCuvAddressableReference>
    {
        [SerializeField] Text _text;
        [SerializeField] Image _image;

        AsyncOperationHandle<Sprite> _handle;
        
        protected override async void OnLoaded()
        {
            Debug.Log("addressables - model: " + ActiveModel.Text);
            _text.text = ActiveModel.Text;
            
            _handle = Addressables.LoadAssetAsync<Sprite>(ActiveModel.Image);
            await _handle.Task;
            if (_handle.Status == AsyncOperationStatus.Succeeded)
            {
                _image.sprite = _handle.Result;
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (_handle.IsValid())
            {
                Addressables.Release(_handle);
            }
        }
    }
}