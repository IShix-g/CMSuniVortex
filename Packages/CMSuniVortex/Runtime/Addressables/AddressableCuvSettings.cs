#if ENABLE_ADDRESSABLES
using System;
using UnityEngine;

namespace CMSuniVortex.Addressable
{
    [Serializable] 
    public struct AddressableCuvSettings
    {
        public AddressableType AddressableType;
        [Tooltip("Create a group with a new name. If not entered, the default name will be used.")]
        public string CustomGroupName;
        
        public string GetGroupName(SystemLanguage language, string className)
            => (string.IsNullOrEmpty(CustomGroupName) ? className : CustomGroupName) + "_" + language;
    }
}
#endif