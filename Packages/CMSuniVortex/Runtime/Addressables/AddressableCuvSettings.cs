#if ENABLE_ADDRESSABLES
using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
#endif

namespace CMSuniVortex.Addressable
{
    [Serializable] 
    public struct AddressableCuvSettings
    {
        public AddressableType AddressableType;
        [Tooltip("Create a group with a new name. If not entered, the default name will be used.")]
        public string CustomGroupName;
#if UNITY_EDITOR
        public BundledAssetGroupSchema.BundleCompressionMode BuildCompressionMode;
        public BundledAssetGroupSchema.BundlePackingMode BundlePackingMode;
        public UpdateRestrictionType UpdateRestriction;
        
        public enum UpdateRestrictionType
        {
            CanChangePostRelease = 0,
            CannotChangePostRelease = 1
        }
#endif
        public string GetGroupName(SystemLanguage language, string className)
            => (string.IsNullOrEmpty(CustomGroupName) ? className : CustomGroupName) + "_" + language;

        static readonly AddressableCuvSettings s_default = new AddressableCuvSettings
        {
            BuildCompressionMode = BundledAssetGroupSchema.BundleCompressionMode.LZ4,
            BundlePackingMode = BundledAssetGroupSchema.BundlePackingMode.PackTogether,
            UpdateRestriction = UpdateRestrictionType.CanChangePostRelease
        };

        public static AddressableCuvSettings Default => s_default;
    }
}
#endif