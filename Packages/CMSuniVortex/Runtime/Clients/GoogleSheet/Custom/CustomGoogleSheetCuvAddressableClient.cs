#if ENABLE_ADDRESSABLES
using System.Collections.Generic;
using CMSuniVortex.Addressable;
using UnityEngine;

namespace CMSuniVortex.GoogleSheet
{
    [CuvClient("Google Sheet")]
    public abstract class CustomGoogleSheetCuvAddressableClient<T, TS> : CustomGoogleSheetCuvClient<T, TS>, IAddressableSettingsProvider where T : CustomGoogleSheetModel, new() where TS : CustomGoogleSheetCuvModelList<T>
    {
        [SerializeField] AddressableCuvSettings _addressableSettings = AddressableCuvSettings.Default;
        
        public void SetSettings(AddressableCuvSettings settings) => _addressableSettings = settings;
        
        protected override void OnStartLoad(string assetPath, IReadOnlyList<SystemLanguage> languages)
        {
#if UNITY_EDITOR
            base.OnStartLoad(assetPath, languages);
            
            foreach (var language in languages)
            {
                var groupName = _addressableSettings.GetGroupName(language, typeof(T).Name);
                if (AddressableHelper.HasGroup(groupName))
                {
                    AddressableHelper.RemoveFromAll(groupName);
                }
                AddressableHelper.CreateGroupIfNotExists(groupName, _addressableSettings, true);
            }
#endif
        }

        protected override void OnLoad(int currentRound, SystemLanguage language, T obj)
        {
#if UNITY_EDITOR
            base.OnLoad(currentRound, language, obj);

            if (obj.AddressableActions != default)
            {
                foreach (var addressableAction in obj.AddressableActions)
                {
                    var groupName = _addressableSettings.GetGroupName(language, typeof(T).Name);
                    var labels = _addressableSettings.GetLocalizedContentsLabels(language);
                    AddressableHelper.AddTo(groupName, addressableAction.Guid, default, labels);
                    addressableAction.Completed(addressableAction.Guid);
                }
            }
#endif
        }

        AddressableCuvSettings IAddressableSettingsProvider.GetSetting() => _addressableSettings;

        void IAddressableSettingsProvider.SetSetting(AddressableCuvSettings settings) => _addressableSettings.Set(settings);
    }
}
#endif