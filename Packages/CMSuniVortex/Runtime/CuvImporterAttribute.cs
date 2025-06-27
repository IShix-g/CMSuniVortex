
using System;
using System.Diagnostics;

namespace CMSuniVortex
{
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class CuvImporterAttribute : Attribute
    {
        public bool IsShowMenu { get; }
        public bool IsShowLogo { get; }
        public bool IsEnabledBuildPath { get; }
        public bool IsEnabledLanguages { get; }
        public bool IsEnabledSelectClient { get; }
        public bool IsEnabledImportButton { get; }
        public bool IsEnabledSelectOutput { get; }
        public bool IsEnabledOutputButton { get; }
        
        public CuvImporterAttribute(
            bool isShowMenu = true,
            bool isShowLogo = true,
            bool isEnabledBuildPath = true,
            bool isEnabledLanguages = true,
            bool isEnabledSelectClient = true,
            bool isEnabledImportButton = true,
            bool isEnabledSelectOutput = true,
            bool isEnabledOutputButton = true)
        {
            IsShowMenu = isShowMenu;
            IsShowLogo = isShowLogo;
            IsEnabledBuildPath = isEnabledBuildPath;
            IsEnabledLanguages = isEnabledLanguages;
            IsEnabledSelectClient = isEnabledSelectClient;
            IsEnabledImportButton = isEnabledImportButton;
            IsEnabledSelectOutput = isEnabledSelectOutput;
            IsEnabledOutputButton = isEnabledOutputButton;
        }
    }
}