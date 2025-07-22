
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
    public abstract class CuvClient<T, TS> 
        : ICuvClient 
        where T : ICuvModel 
        where TS : ScriptableObject, ICuvModelList<T>, ICuvModelListSetter<T>
    {
        public virtual bool CanILoad()
        {
            if (GetRepeatCount() <= 0)
            {
                Debug.LogError("Set RoundCount to 1 or more.");
                return false;
            }
            return true;
        }
        
        protected abstract IEnumerator LoadModels(
            int currentRound,
            string buildPath,
            string cuvId,
            Action<T[], string> onSuccess = default,
            Action<string> onError = default
        );
        
        public abstract IReadOnlyList<string> GetCuvIds();
        
        public virtual int GetRepeatCount() => 1;
        public virtual string GetKeyName() => "Key";
        protected virtual void OnStartLoad(string assetPath, IReadOnlyList<string> cuvIds) {}
        protected virtual void OnLoad(int currentRound, string cuvId, T obj) {}
        protected virtual void OnLoad(int currentRound, string guid, TS obj) {}
        protected virtual void OnLoaded(string[] guids, TS[] objs) {}
        protected virtual void OnSelect(string assetPath){}
        protected virtual void OnDeselect(){}
        
        public IEnumerator Load(string buildPath, Action<string[]> onLoaded)
        {
#if UNITY_EDITOR
            var cuvIds = GetCuvIds();

            OnStartLoad(buildPath, cuvIds);

            var objs = new TS[cuvIds.Count * GetRepeatCount()];
            var guids = new string[cuvIds.Count * GetRepeatCount()];

            for (var s = 0; s < GetRepeatCount(); s++)
            {
                var currentRound = s + 1;
                for (var i = 0; i < cuvIds.Count; i++)
                {
                    var index = i;
                    var cuvId = cuvIds[i];
                    yield return LoadModels(
                        currentRound,
                        buildPath,
                        cuvId,
                        (models, objFileName) =>
                    {
                        foreach (var model in models)
                        {
                            OnLoad(currentRound, cuvId, model);
                            if (model is IDeserializationNotifier notifier)
                            {
                                notifier.OnDeserialized();
                            }
                        }
                        
                        var path = Path.Combine(buildPath, objFileName + ".asset");
                        var obj = default(TS);
                        if (File.Exists(path))
                        {
                            obj = AssetDatabase.LoadAssetAtPath<TS>(path);
                            Debug.Log("Loading CuvModelList in " + cuvId + " path: " + path);
                        }
                        if(obj == default)
                        {
                            obj = ScriptableObject.CreateInstance<TS>();
                            obj.hideFlags = HideFlags.NotEditable;
                            AssetDatabase.CreateAsset(obj, path);
                            Debug.Log("Generation of CuvModelList in " + cuvId + " path: " + path);
                        }

                        var objIndex = (currentRound - 1) * 2 + index;
                        obj.name = objFileName;
                        obj.SetData(cuvId, models);
                        objs[objIndex] = obj;
                        var guid = AssetDatabase.AssetPathToGUID(path);
                        guids[objIndex] = guid;
                        OnLoad(currentRound, guid, obj);
                        AssetDatabase.SaveAssetIfDirty(obj);
                    });
                }
            }

            OnLoaded(guids, objs);
            onLoaded?.Invoke(guids);
#else
        return default;
#endif
        }

        public void Select(string assetPath) => OnSelect(assetPath);

        public void Deselect() => OnDeselect();
    }
}