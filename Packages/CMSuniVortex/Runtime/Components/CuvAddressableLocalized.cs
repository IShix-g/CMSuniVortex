#if ENABLE_ADDRESSABLES
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CMSuniVortex
{
    public abstract class CuvAddressableLocalized<T> : MonoBehaviour where T : Object, ICuvLocalizedAsyncReference
    {
        [SerializeField] T _reference;
        [SerializeField, CuvModelKey("_reference")] string _key;

        public T Reference => _reference;

        protected abstract void OnChangeLanguage(T reference, string key);
        
        protected virtual void OnEnable()
        {
            Assert.IsFalse(string.IsNullOrEmpty(_key), "The key value is empty, please set it.");
            
            _reference.OnLoadedLanguage += OnChangeLanguage;
            if (_reference.IsInitializedLocalize)
            {
                OnChangeLanguage(_reference, _key);
            }
        }

        protected virtual void OnDisable() => _reference.OnLoadedLanguage -= OnChangeLanguage;
        
        public void ChangeLanguage(SystemLanguage language) => _reference.ChangeLanguage(language);
        
        void OnChangeLanguage(SystemLanguage language)
        {
            if (!string.IsNullOrEmpty(_key))
            {
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
#endif