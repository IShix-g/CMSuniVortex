
using System;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CMSuniVortex.Compornents
{
    public abstract class CuvComponent<T> : MonoBehaviour where T : ScriptableObject, ICuvReference
    {
        [SerializeField] T _reference;
        [SerializeField, CuvModelKey("_reference")] string _key;

        public T Reference => _reference;
        
        SystemLanguage? _current;

        protected abstract void OnChangeLanguage(T reference, string key);
        
        protected virtual void OnEnable()
        {
#if DEBUG
            if (string.IsNullOrEmpty(_key))
            {
                throw new InvalidCastException("The key value is empty, please set it.");
            }
#endif
            _reference.OnChangeLanguage += OnChangeLanguage;
            if (!_current.HasValue
                || _current != _reference.Language)
            {
                OnChangeLanguage(_reference.Language);
            }
        }

        protected virtual void OnDisable() => _reference.OnChangeLanguage -= OnChangeLanguage;

        public void ChangeLanguage(SystemLanguage language)
            => _reference.ChangeLanguage(language);
        
        void OnChangeLanguage(SystemLanguage language)
        {
            if (!string.IsNullOrEmpty(_key))
            {
                _current = _reference.Language;
                OnChangeLanguage(_reference, _key);
            }
        }
        
        protected virtual void Reset()
        {
#if UNITY_EDITOR
            _reference = AssetDatabase.FindAssets("t:" + typeof(ScriptableObject))
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<ScriptableObject>)
                .FirstOrDefault(x => x is T) as T;
#endif
        }
    }
}