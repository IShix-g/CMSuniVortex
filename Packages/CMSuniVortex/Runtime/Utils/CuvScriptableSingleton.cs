
using System.IO;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CMSuniVortex
{
    public abstract class CuvScriptableSingleton<T> : ScriptableObject where T : CuvScriptableSingleton<T>
    {
        static T s_instance;

        public static T Instance
        {
            get
            {
                if (s_instance == default)
                {
                    var filePath = GetFilePath();
                    Load(filePath);
                }
                return s_instance;
            }
        }

        public static bool HasSettings => Instance != default;
        
        public static void CreateAndLoad()
        {
            var filePath = GetFilePath();
#if UNITY_EDITOR
            if (!File.Exists(filePath))
            {
                 s_instance = CreateInstance<T>();
                 s_instance.Save();
            }
#endif
            Load(filePath);
        }

        static void Load(string filePath)
        {
            var resourcePath = Path.HasExtension(filePath)
                    ? Path.GetFileNameWithoutExtension(filePath)
                    : filePath;
            s_instance = Resources.Load<T>(resourcePath);
        }

        protected virtual void Save()
        {
#if UNITY_EDITOR
            if (s_instance == default)
            {
                Debug.LogError("Cannot save ScriptableSingleton: no instance!");
            }
            else
            {
                var filePath = GetFilePath();
                var directoryName = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directoryName)
                    && !Directory.Exists(directoryName))
                {
                    Directory.CreateDirectory(directoryName);
                }
                AssetDatabase.CreateAsset(s_instance, filePath);
                AssetDatabase.SaveAssets();
            }
#endif
        }

        protected static string GetFilePath()
        {
            foreach (var customAttribute in typeof (T).GetCustomAttributes(true))
            {
                if (customAttribute is CuvScriptableObjectPathAttribute attribute)
                {
                    return attribute.FilePath;
                }
            }
            return string.Empty;
        }
    }
}