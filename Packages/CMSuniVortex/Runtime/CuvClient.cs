
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
        protected abstract IEnumerator LoadModels(
            int currentRound,
            string buildPath,
            SystemLanguage language,
            Action<T[], string> onSuccess = default,
            Action<string> onError = default
        );

        public virtual bool CanILoad()
        {
            if (GetRepeatCount() < 1)
            {
                Debug.LogError("Set RoundCount to 1 or more.");
                return false;
            }
            return true;
        }

        public virtual int GetRepeatCount() => 1;

        protected virtual void OnStartLoad(string assetPath, IReadOnlyList<SystemLanguage> languages) {}

        protected virtual void OnLoad(int currentRound, string guid, TS obj) {}

        protected virtual void OnLoaded(string[] guids, TS[] objs) {}

        public IEnumerator Load(string buildPath, IReadOnlyList<SystemLanguage> languages, Action<string[]> onLoaded)
        {
#if UNITY_EDITOR
            OnStartLoad(buildPath, languages);

            var objs = new TS[languages.Count * GetRepeatCount()];
            var guids = new string[languages.Count * GetRepeatCount()];

            for (var s = 0; s < GetRepeatCount(); s++)
            {
                var currentRound = s + 1;
                for (var i = 0; i < languages.Count; i++)
                {
                    var index = i;
                    var language = languages[i];
                    yield return LoadModels(currentRound, buildPath, language, (models, objFileName) =>
                    {
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

                        var objIndex = (currentRound - 1) * 2 + index;
                        obj.name = objFileName;
                        obj.SetData(language, models);
                        AssetDatabase.SaveAssetIfDirty(obj);
                        objs[objIndex] = obj;
                        var guid = AssetDatabase.AssetPathToGUID(path);
                        guids[objIndex] = guid;
                        OnLoad(currentRound, guid, obj);
                    });
                }
            }

            OnLoaded(guids, objs);
            onLoaded?.Invoke(guids);
#else
        return default;
#endif
        }
    }
}