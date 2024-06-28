#if ENABLE_ADDRESSABLES
using System.Collections.Generic;
using CMSuniVortex.Addressable;
using UnityEngine;

namespace CMSuniVortex.Cockpit
{
    public abstract class CockpitCuvAddressableClient<T, TS> : CockpitCuvClient<T, TS>, IAddressableSettingsProvider where T : CockpitModel where TS : CockpitCuvModelList<T>
    {
        [SerializeField] AddressableCuvSettings _addressableSettings = AddressableCuvSettings.Default;

        public void SetSettings(AddressableCuvSettings settings) => _addressableSettings = settings;
        
        protected override void OnStartLoad(string assetPath, IReadOnlyList<SystemLanguage> languages)
        {
#if UNITY_EDITOR
            base.OnStartLoad(assetPath, languages);
            
            foreach (var language in languages)
            {
                AddressableHelper.CreateGroupIfNotExists(_addressableSettings.GetGroupName(language, typeof(T).Name), _addressableSettings, true);
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
                    AddressableHelper.AddTo(_addressableSettings.GetGroupName(language, typeof(T).Name), t.Guid, default, _addressableSettings.Labels);
                    t.Completed(t.Guid);
                }
            }
#endif
        }

        AddressableCuvSettings IAddressableSettingsProvider.GetSetting() => _addressableSettings;

        void IAddressableSettingsProvider.SetSetting(AddressableCuvSettings settings) => _addressableSettings.Set(settings);
    }
}
#endif