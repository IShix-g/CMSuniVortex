
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CMSuniVortex.Editor
{
    public sealed class ScriptGeneratorContextMenu
    {
        [MenuItem("Assets/Create/CMSuniVortex/Generation of required classes.", true)]
        static bool CanIShowDialog()
        {
            if (Selection.activeObject is MonoScript monoScript)
            {
                var type = monoScript.GetClass();
                return type != default
                       && monoScript.GetClass().IsClass
                       && !monoScript.GetClass().IsAbstract
                       && monoScript.GetClass().IsPublic;
            }
            return false;
        }
        
        [MenuItem("Assets/Create/CMSuniVortex/Generation of required classes.", false, 50)]
        static void ShowDialog()
        {
            if (Selection.activeObject is MonoScript monoScript
                && monoScript.GetClass().GetInterfaces().Any(type => type == typeof(ICuvModel)))
            {
                var assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
                if (Path.HasExtension(assetPath))
                {
                    assetPath = Path.GetDirectoryName(assetPath);
                }
                ScriptGeneratorWindow.ShowDialog(monoScript.GetClass().FullName, assetPath);
            }
            else 
            {
                Debug.LogWarning("Select a class derived from ICuvModel.");
            }
        }
    }
}