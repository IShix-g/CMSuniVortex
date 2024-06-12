
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;
using UnityEngine;

namespace CMSuniVortex
{
    /// <summary>
    /// Represents a generic abstract class that manages a list of models.
    /// </summary>
    public abstract class CuvModelList<T> : ScriptableObject, ICuvModelList where T : ICuvModel
    {
        [SerializeField] SystemLanguage _language;
        [SerializeField] T[] _models;

        public SystemLanguage Language => _language;
        public int Length => _models.Length;
        
        protected virtual void OnSetData(SystemLanguage language, IReadOnlyList<T> models){}
        
        [Conditional("UNITY_EDITOR")]
        public void SetData(SystemLanguage language, T[] models)
        {
            _language = language;
            _models = models;
            OnSetData(_language, _models);
        }
        
        public T GetByIndex(int index) => _models[index];

        [CanBeNull]
        public T GetById(string id)
        {
            foreach (var model in _models)
            {
                if (model.GetID() == id)
                {
                    return model;
                }
            }
            return default;
        }
        
        public bool TryGetById(string id, out T model)
        {
            foreach (var m in _models)
            {
                if (m.GetID() == id)
                {
                    model = m;
                    return true;
                }
            }
            model = default;
            return false;
        }
    }
}