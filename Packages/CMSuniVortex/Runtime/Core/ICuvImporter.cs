
using System;
using UnityEngine;

namespace CMSuniVortex
{
    /// <summary>
    /// CuvImporter is responsible for importing CMS data into Unity.
    /// </summary>
    public interface ICuvImporter
    {
        public bool IsBuildCompleted { get; }
        public SystemLanguage[] Languages { get; }
        public string[] ModelListGuilds { get; }
        public bool IsLoading { get; }
        bool CanImport();
        void StartImport(Action onLoaded = default);
    }
}