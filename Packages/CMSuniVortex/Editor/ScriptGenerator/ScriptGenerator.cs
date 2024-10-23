
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace CMSuniVortex.Editor
{
    public abstract class ScriptGenerator
    {
        public abstract string GetName();
        public abstract string GetLogoName();
        protected abstract IEnumerable<(string Path, string Text)> OnGenerate(string namespaceName, string className, string rootPath, bool isGenerateOutput);
        
        public void Generate(string className, string rootPath, bool isGenerateOutput)
        {
            var isChanged = false;
            var namespaceName = string.Empty;
            var lastDotIndex = className.LastIndexOf('.');
            
            if (lastDotIndex != -1)
            {
                namespaceName = className.Substring(0, lastDotIndex);
                className = className.Substring(lastDotIndex + 1);
            }
            
            foreach (var result in OnGenerate(namespaceName, className, rootPath, isGenerateOutput))
            {
                var changed = true;
                CreateDirectory(result.Path);
                if (File.Exists(result.Path))
                {
                    var current = File.ReadAllText(result.Path);
                    changed = current != result.Text;
                }
                if (changed)
                {
                    File.WriteAllText(result.Path, result.Text);
                }
                isChanged |= changed;
                Debug.Log("Script Generation path:" + result.Path);
            }
            
            if (isChanged)
            {
                AssetDatabase.Refresh();
                AssetDatabase.SaveAssets();
            }
            Debug.Log("Script Generation Completed. isChanged:" + isChanged);
        }
        
        [CanBeNull]
        internal Texture2D GetLogo()
        {
            var logoName = GetLogoName();
            if (!string.IsNullOrEmpty(logoName))
            {
                var guids = AssetDatabase.FindAssets("t:Texture2D " + logoName);
                foreach (var guid in guids)
                {
                    var path = AssetDatabase.GUIDToAssetPath(guid);
                    return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                }
            }
            return default;
        }
        
        public void CreateDirectory(string path)
        {
            if (!path.StartsWith("Assets/"))
            {
                return;
            }
            if (!string.IsNullOrEmpty(Path.GetExtension(path)))
            {
                path = Path.GetDirectoryName(path);
            }
            if (string.IsNullOrEmpty(path))
            {
                return;
            }
            var dirs = path.Split('/');
            var combinePath = dirs[0];
            foreach (var dir in dirs.Skip(1))
            {
                if (!AssetDatabase.IsValidFolder(combinePath + '/' + dir))
                {
                    AssetDatabase.CreateFolder(combinePath, dir);
                }
                combinePath += '/' + dir;
            }
        }
    }
}