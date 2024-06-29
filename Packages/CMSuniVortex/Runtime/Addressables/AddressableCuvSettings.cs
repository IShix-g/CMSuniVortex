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
        public string[] Labels;
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

        public void Set(AddressableCuvSettings settings)
        {
            AddressableType = settings.AddressableType;
            CustomGroupName = settings.CustomGroupName;
            if (settings.Labels != null)
            {
                Labels = new string[settings.Labels.Length];
                settings.Labels.CopyTo(Labels, 0);
            }
            else
            {
                Labels = null;
            }
#if UNITY_EDITOR
            BuildCompressionMode = settings.BuildCompressionMode;
            BundlePackingMode = settings.BundlePackingMode;
            UpdateRestriction = settings.UpdateRestriction;
#endif
        }
        
        public string GetGroupName(SystemLanguage language, string className)
            => (string.IsNullOrEmpty(CustomGroupName) ? className : CustomGroupName) + "_" + language;
        
        static readonly AddressableCuvSettings s_default = new AddressableCuvSettings
        {
#if UNITY_EDITOR
            BuildCompressionMode = BundledAssetGroupSchema.BundleCompressionMode.LZ4,
            BundlePackingMode = BundledAssetGroupSchema.BundlePackingMode.PackTogether,
            UpdateRestriction = UpdateRestrictionType.CanChangePostRelease
#endif
        };

        public static AddressableCuvSettings Default => s_default;
    }
}
#endif