#if ENABLE_ADDRESSABLES
using System.Collections.Generic;
using CMSuniVortex.Addressable;
using UnityEngine;

namespace CMSuniVortex.Cockpit
{
    public abstract class CockpitCuvAddressableClient<T, TS> : CockpitCuvClient<T, TS>, IAddressableSettingsProvider where T : CockpitModel where TS : CockpitCuvModelList<T>
    {
        [SerializeField] AddressableCuvSettings _addressableSettings;
        
        protected override void OnStartLoad(string assetPath, IReadOnlyList<SystemLanguage> languages)
        {
            base.OnStartLoad(assetPath, languages);
            
            foreach (var language in languages)
            {
                AddressableHelper.CreateGroupIfNotExists(_addressableSettings.GetGroupName(language), _addressableSettings.AddressableType, true);
            }
        }

        protected override void OnLoad(int currentRound, SystemLanguage language, T obj)
        {
            base.OnLoad(currentRound, language, obj);

            if (obj.AddressableActions != default)
            {
                foreach (var t in obj.AddressableActions)
                {
                    AddressableHelper.AddTo(_addressableSettings.GetGroupName(language), t.Guid);
                    t.Completed(t.Guid);
                }
            }
        }

        AddressableCuvSettings IAddressableSettingsProvider.GetSetting() => _addressableSettings;

        void IAddressableSettingsProvider.SetSetting(AddressableCuvSettings settings)
        {
            _addressableSettings = settings;
        }
    }
}
#endif