#if ENABLE_ADDRESSABLES
using System.Collections.Generic;
using CMSuniVortex.Addressable;
using UnityEngine;

namespace CMSuniVortex.Cockpit
{
    public abstract class CockpitCuvAddressableLocalizedClient<T, TS> 
        : CockpitCuvLocalizedClient<T, TS>, IAddressableSettingsProvider 
        where T : CockpitModel 
        where TS : CockpitCuvModelList<T>
    {
        [SerializeField] AddressableCuvSettings _addressableSettings = AddressableCuvSettings.Default;

        public void SetSettings(AddressableCuvSettings settings) => _addressableSettings = settings;
        
        protected override void OnStartLoad(string assetPath, IReadOnlyList<string> cuvIds)
        {
#if UNITY_EDITOR
            base.OnStartLoad(assetPath, cuvIds);
            
            foreach (var language in cuvIds)
            {
                var groupName = _addressableSettings.GetGroupName(language, typeof(T).Name);
                AddressableHelper.CreateGroupIfNotExists(groupName, _addressableSettings, true);
            }
#endif
        }

        protected override void OnLoad(int currentRound, string cuvId, T obj)
        {
#if UNITY_EDITOR
            base.OnLoad(currentRound, cuvId, obj);

            if (obj.AddressableActions != default)
            {
                foreach (var addressableAction in obj.AddressableActions)
                {
                    var groupName = _addressableSettings.GetGroupName(cuvId, typeof(T).Name);
                    var labels = _addressableSettings.GetLocalizedContentsLabels(cuvId);
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