
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace CMSuniVortex
{
    /// <summary>
    /// Represents a generic abstract class that manages a list of models.
    /// </summary>
    public abstract class CuvModelList<T> : ScriptableObject, ICuvModelList<T>, ICuvModelListSetter<T> where T : ICuvModel
    {
        [SerializeField] SystemLanguage _language;
        [SerializeField] T[] _models;
        
        public SystemLanguage Language => _language;
        public int Length => _models.Length;

        protected virtual void OnSetData(SystemLanguage language, IReadOnlyList<T> models){}
        
        void ICuvModelListSetter<T>.SetData(SystemLanguage language, T[] models)
        {
            _language = language;
            _models = models;
            OnSetData(_language, _models);
        }

        public T GetByIndex(int index) => _models[index];

        [CanBeNull]
        public T GetByKey(string id)
        {
            foreach (var model in _models)
            {
                if (model.GetKey() == id)
                {
                    return model;
                }
            }
            return default;
        }
        
        public bool TryGetByKey(string id, out T model)
        {
            foreach (var m in _models)
            {
                if (m.GetKey() == id)
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