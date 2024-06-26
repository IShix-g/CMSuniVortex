#if ENABLE_ADDRESSABLES
using UnityEngine;

namespace CMSuniVortex.Addressable
{
    public abstract class CuvAddressableOutput<TModel, TModelList, TReference> : ICuvOutput, IAddressableSettingsProvider
        where TModel : ICuvModel, IAddressableModel
        where TModelList : ICuvModelList<TModel>
        where TReference : ICuvAsyncReference
    {
        [SerializeField] AddressableCuvSettings _addressableSettings = AddressableCuvSettings.Default;
        
        public AddressableCuvSettings AddressableSettings => _addressableSettings;
        
        public void SetSettings(AddressableCuvSettings settings) => _addressableSettings = settings;
        
        public abstract void Select(string assetPath);
        public abstract void Deselect();
        public abstract void Generate(string buildPath, ICuvClient client, string[] listGuids);
        public abstract void Release();

        public string GetGroupName(SystemLanguage language) => _addressableSettings.GetGroupName(language, typeof(TModel).Name);

        AddressableCuvSettings IAddressableSettingsProvider.GetSetting() => _addressableSettings;

        void IAddressableSettingsProvider.SetSetting(AddressableCuvSettings settings) => _addressableSettings = settings;
    }
}
#endif