#if ENABLE_ADDRESSABLES
using System.Collections.Generic;
using CMSuniVortex.Addressable;
using UnityEngine;

namespace CMSuniVortex.GoogleSheet
{
    public abstract class CustomGoogleSheetCuvAddressableClient<T, TS> : CustomGoogleSheetCuvClient<T, TS>, IAddressableSettingsProvider where T : CustomGoogleSheetModel, new() where TS : CustomGoogleSheetCuvModelList<T>
    {
        [SerializeField] AddressableCuvSettings _addressableSettings;
        
        protected override void OnStartLoad(string assetPath, IReadOnlyList<SystemLanguage> languages)
        {
#if UNITY_EDITOR
            base.OnStartLoad(assetPath, languages);
            
            foreach (var language in languages)
            {
                AddressableHelper.CreateGroupIfNotExists(_addressableSettings.GetGroupName(language, typeof(T).Name), _addressableSettings.AddressableType, true);
            }
#endif
        }

        protected override void OnLoad(int currentRound, SystemLanguage language, T obj)
        {
#if UNITY_EDITOR
            base.OnLoad(currentRound, language, obj);

            if (obj.AddressableActions != default)
            {
                foreach (var t in obj.AddressableActions)
                {
                    AddressableHelper.AddTo(_addressableSettings.GetGroupName(language, typeof(T).Name), t.Guid);
                    t.Completed(t.Guid);
                }
            }
#endif
        }

        AddressableCuvSettings IAddressableSettingsProvider.GetSetting() => _addressableSettings;

        void IAddressableSettingsProvider.SetSetting(AddressableCuvSettings settings)
        {
            _addressableSettings = settings;
        }
    }
}
#endif