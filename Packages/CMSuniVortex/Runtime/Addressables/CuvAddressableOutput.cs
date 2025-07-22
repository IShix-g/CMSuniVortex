#if ENABLE_ADDRESSABLES
using UnityEngine;
using Object = UnityEngine.Object;

namespace CMSuniVortex.Addressable
{
    public abstract class CuvAddressableOutput<TModel, TModelList, TReference> : ICuvOutput, IAddressableSettingsProvider
        where TModel : ICuvModel, IAddressableModel
        where TModelList : Object, ICuvModelList<TModel>
        where TReference : ICuvAsyncReference<TModel, TModelList>
    {
        [SerializeField] AddressableCuvSettings _addressableSettings = AddressableCuvSettings.Default;
        
        public AddressableCuvSettings AddressableSettings => _addressableSettings;
        
        public void SetSettings(AddressableCuvSettings settings) => _addressableSettings = settings;

        public abstract TReference GetReference();
        public abstract bool IsCompleted();
        public abstract void Select(string assetPath);
        public abstract void Deselect();
        public abstract void Generate(string buildPath, ICuvClient client, string[] listGuids);
        public abstract void Release();
        public abstract void ReloadReference(string buildPath);

        public string GetGroupName(string cuvId) => _addressableSettings.GetGroupName(cuvId, typeof(TModel).Name);

        AddressableCuvSettings IAddressableSettingsProvider.GetSetting() => _addressableSettings;

        void IAddressableSettingsProvider.SetSetting(AddressableCuvSettings settings) => _addressableSettings.Set(settings);
    }
}
#endif