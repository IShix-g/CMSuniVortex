#if ENABLE_ADDRESSABLES
using System;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
#endif

namespace CMSuniVortex.Addressable
{
    [Serializable] 
    public struct AddressableCuvSettings
    {
        public const string LanguagePlaceholder = "[language]";
        
        public AddressableType AddressableType;
        [Tooltip("Create a group with a new name. If not entered, the default name will be used.")]
        public string CustomGroupName;
        public string[] ListLabels;
        public string[] ContentsLabels;
        
#if UNITY_EDITOR
        public BundledAssetGroupSchema.BundleCompressionMode BuildCompressionMode;
        public BundledAssetGroupSchema.BundlePackingMode BundlePackingMode;
        public UpdateRestrictionType UpdateRestriction;
        public int RetryCount;
        
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

            if (settings.ListLabels != null)
            {
                ListLabels = new string[settings.ListLabels.Length];
                settings.ListLabels.CopyTo(ListLabels, 0);
            }
            else
            {
                ListLabels = null;
            }
            
            if (settings.ContentsLabels != null)
            {
                ContentsLabels = new string[settings.ContentsLabels.Length];
                settings.ContentsLabels.CopyTo(ContentsLabels, 0);
            }
            else
            {
                ContentsLabels = null;
            }
            
#if UNITY_EDITOR
            BuildCompressionMode = settings.BuildCompressionMode;
            BundlePackingMode = settings.BundlePackingMode;
            UpdateRestriction = settings.UpdateRestriction;
            RetryCount = settings.RetryCount;
#endif
        }
        
        public string GetGroupName(SystemLanguage language, string className)
            => (string.IsNullOrEmpty(CustomGroupName) ? className : CustomGroupName) + "_" + language;

        public string[] GetLocalizedListLabels(SystemLanguage language)
        {
            var languageString = language.ToString();
            return ListLabels.Any(label => label.Contains(LanguagePlaceholder))
                ? ListLabels.Select(label => label.Contains(LanguagePlaceholder)
                    ? label.Replace(LanguagePlaceholder, languageString)
                    : label).ToArray()
                : ListLabels;
        }
        
        public string[] GetLocalizedContentsLabels(SystemLanguage language)
        {
            var languageString = language.ToString();
            return ContentsLabels.Any(label => label.Contains(LanguagePlaceholder))
                ? ContentsLabels.Select(label => label.Contains(LanguagePlaceholder)
                    ? label.Replace(LanguagePlaceholder, languageString)
                    : label).ToArray()
                : ContentsLabels;
        }
        
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