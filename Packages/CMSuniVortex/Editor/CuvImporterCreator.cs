
using System.IO;
using UnityEditor;
using UnityEngine;

namespace CMSuniVortex.Editor
{
    internal sealed class CuvImporterCreator
    {
        [MenuItem("Window/CMSuniVortex/create CuvImporter", false, 1)]
        static void CreateMyAsset()
        {
            var path = EditorUtility.SaveFilePanelInProject("Save Location", "New " + nameof(CuvImporter), "asset", "Please enter a file name to save the asset to");

            if (string.IsNullOrEmpty(path)
                || File.Exists(path))
            {
                return;
            }
            
            var asset = ScriptableObject.CreateInstance<CuvImporter>();
            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();
            EditorUtility.FocusProjectWindow();
            var obj = AssetDatabase.LoadAssetAtPath<Object>(path);
            Selection.activeObject = obj;
            EditorGUIUtility.PingObject(obj);
        }
    }
}