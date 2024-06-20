#if ENABLE_ADDRESSABLES
using System;
using UnityEngine;

namespace CMSuniVortex.Addressable
{
    [Serializable] 
    public struct AddressableCuvSettings
    {
        public const string DefaultPrefix = "Cuv_";
        
        public AddressableType AddressableType;
        [Tooltip("Create a group with a new name. If not entered, the default name will be used.")]
        public string CustomGroupName;
        
        public string GetGroupName(SystemLanguage language)
            => (string.IsNullOrEmpty(CustomGroupName) ? DefaultPrefix : CustomGroupName + "_") + language;
    }
}
#endif