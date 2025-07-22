
using System;

namespace CMSuniVortex
{
    /// <summary>
    /// CuvImporter is responsible for importing CMS data into Unity.
    /// </summary>
    public interface ICuvImporter
    {
        public bool IsBuildCompleted { get; }
        public string BuildPath { get; }
        public string[] ModelListGuilds { get; }
        public bool IsLoading { get; }
        bool CanIImport();
        void StartImport(Action onLoaded = default);
        void SelectClient();
        void DeselectClient();
        bool CanIOutput();
        void StartOutput();
        void SelectOutput();
        void DeselectOutput();
    }
}