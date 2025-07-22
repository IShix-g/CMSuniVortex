
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CMSuniVortex.Editor
{
    public static class CuvImportersCache
    {
        static CuvImporter[] s_importers;
        static CuvImportersView.IconState[] s_iconStates;

        public static CuvImportersView.IconState[] IconStates => s_iconStates;

        static CuvImportersCache() => ReImport();
        
        public static void ReImport()
        {
            s_importers = AssetDatabase.FindAssets("t:ScriptableObject")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<ScriptableObject>)
                .OfType<CuvImporter>()
                .ToArray();

            LoadIconStatus();
        }

        static void LoadIconStatus()
        {
            s_iconStates = ScriptGenerator.Generators
                .Where(g => !string.IsNullOrEmpty(g.GetLogoName()))
                .Select(g => new CuvImportersView.IconState(g.GetName(), g.GetLogo()))
                .ToArray();
        }
        
        public static T[] FilterLocalizedImporters<T>()
            => s_importers
                .Where(x => x.Client is ICuvLocalizedClient)
                .OfType<T>()
                .ToArray();
        
        public static T[] FilterImporters<T>()
            => s_importers
                .OfType<T>()
                .ToArray();

        public static T[] FilterClients<T>()
            => s_importers
                .Select(x => x.Client)
                .OfType<T>()
                .ToArray();
        
        public static T[] FilterOutputs<T>()
            => s_importers
                .Select(x => x.Output)
                .OfType<T>()
                .ToArray();
    }
}