
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace CMSuniVortex
{
    /// <summary>
    /// Represents a generic abstract class that manages a list of models.
    /// </summary>
    public abstract class CuvModelList<T> : ScriptableObject, IReadOnlyList<T>, ICuvModelList<T>, ICuvModelListSetter<T> where T : ICuvModel
    {
        [SerializeField] string _cuvId;
        [SerializeField] T[] _models;

        public string CuvId => _cuvId;
        public int Length => _models.Length;

        protected virtual void OnSetData(string cuvId, IReadOnlyList<T> models){}
        
        void ICuvModelListSetter<T>.SetData(string cuvId, T[] models)
        {
            _cuvId = cuvId;
            _models = models;
            OnSetData(_cuvId, _models);
        }

        public T GetFirst() => _models[0];
        
        public T GetLast() => _models[^1];
        
        public T GetAt(int index) => _models[index];

        [CanBeNull]
        public T GetByKey(string key)
        {
            foreach (var model in _models)
            {
                if (model.GetKey() == key)
                {
                    return model;
                }
            }
            return default;
        }
        
        public bool TryGetByKey(string key, out T model)
        {
            foreach (var m in _models)
            {
                if (m.GetKey() == key)
                {
                    model = m;
                    return true;
                }
            }
            model = default;
            return false;
        }

        public int Count => Length;

        public T this[int index] => GetAt(index);
        
        public IEnumerator<T> GetEnumerator()
        {
            foreach (var model in _models)
            {
                yield return model;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}