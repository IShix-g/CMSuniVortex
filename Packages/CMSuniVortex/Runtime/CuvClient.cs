
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CMSuniVortex
{
    /// <summary>
    /// Represents a generic abstract client class that manages loading and serialization of model data.
    /// </summary>
    public abstract class CuvClient<T, TS> : ICuvClient where T : ICuvModel where TS : CuvModelList<T>
    {
        public abstract bool CanILoad();
        protected abstract IEnumerator LoadModels(string buildPath, SystemLanguage language, Action<T[]> onSuccess = default, Action<string> onError = default);

        protected virtual void OnStartLoad(string assetPath, IReadOnlyList<SystemLanguage> languages) {}
        protected virtual void OnLoad(string guid, TS obj) {}
        protected virtual void OnLoaded(string[] guids, TS[] objs) {}

        public IEnumerator Load(string buildPath, IReadOnlyList<SystemLanguage> languages, Action<string[]> onLoaded)
        {
#if UNITY_EDITOR
            OnStartLoad(buildPath, languages);

            var guids = new string[languages.Count];
            var objs = new TS[languages.Count];
            var fileName = typeof(TS).Name;

            for (var i = 0; i < languages.Count; i++)
            {
                var index = i;
                var language = languages[i];
                yield return LoadModels(buildPath, language, models =>
                {
                    var objFileName = fileName + "_" + language;
                    var path = Path.Combine(buildPath, objFileName + ".asset");
                    var obj = default(TS);
                    if (File.Exists(path))
                    {
                        obj = AssetDatabase.LoadAssetAtPath<TS>(path);
                    }
                    else
                    {
                        obj = ScriptableObject.CreateInstance<TS>();
                        obj.hideFlags = HideFlags.NotEditable;
                        AssetDatabase.CreateAsset(obj, path);
                    }

                    obj.name = objFileName;
                    obj.SetData(language, models);
                    AssetDatabase.SaveAssetIfDirty(obj);
                    objs[index] = obj;
                    var guid = AssetDatabase.AssetPathToGUID(path);
                    guids[index] = guid;
                    OnLoad(guid, obj);
                });
            }

            OnLoaded(guids, objs);
            onLoaded?.Invoke(guids);
#else
        return default;
#endif
        }
    }
}