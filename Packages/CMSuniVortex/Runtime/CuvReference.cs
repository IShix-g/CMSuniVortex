
using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CMSuniVortex
{
    /// <summary>
    /// Managing a reference to a list of models.
    /// </summary>
    public abstract class CuvReference<T, TS> : ScriptableObject, ICuvReference where T : ICuvModel where TS : ICuvModelList<T>
    {
        public event Action<SystemLanguage> OnChangeLanguage = delegate {};
        
        [SerializeField] TS[] _modelLists;

        public int ContentsLength => GetList().Length;

        public SystemLanguage Language { get; private set; }
        public TS[] ModelLists => _modelLists;
        
        int _currentLanguageIndex;

        protected virtual void OnEnable()
        {
            if (_modelLists is {Length: > 0})
            {
                ChangeLanguage(Application.systemLanguage);
            }
            
#if UNITY_EDITOR
            if( EditorApplication.isPlayingOrWillChangePlaymode )
            {
                EditorApplication.playModeStateChanged += LogPlayModeState;
            }
#endif
        }
        
#if UNITY_EDITOR
        void LogPlayModeState(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingPlayMode)
            {
                EditorApplication.playModeStateChanged -= LogPlayModeState;
                Resources.UnloadAsset(this);
            }
        }
#endif
        
        public void SetModelLists(TS[] modelLists) => _modelLists = modelLists;

        public TS GetList() => _modelLists[_currentLanguageIndex];

        public T GetById(string id)
        {
            if (GetList().TryGetByKey(id, out var model))
            {
                return model;
            }
            throw new ArgumentException("Id that does not exist : " + id);
        }
        
        public bool TryGetById(string id, out T model)
        {
            model = GetList().GetByKey(id);
            return model != null;
        }

        public T GetByIndex(int index)
            => GetList().GetByIndex(index);
        
        public void ChangeLanguage(SystemLanguage language)
        {
            for (var i = 0; i < _modelLists.Length; i++)
            {
                var list = _modelLists[i];
                if (list.Language == language)
                {
                    _currentLanguageIndex = i;
                    SetLanguage(language);
                    return;
                }
            }
            var defaultLang = _modelLists[0].Language;
            _currentLanguageIndex = 0;
            SetLanguage(defaultLang);
        }

        public bool HasContents()
            => _modelLists is {Length: > 0}
               && _modelLists[0].Length > 0;

        public string[] GetKeys()
        {
            if (_modelLists == default
                || _modelLists.Length == 0)
            {
                return Array.Empty<string>();
            }
            var list = _modelLists[0];
            var keys = new string[list.Length];
            for (var i = 0; i < list.Length; i++)
            {
                keys[i] = list.GetByIndex(i).GetKey();
            }
            return keys;
        }

        void SetLanguage(SystemLanguage newLanguage)
        {
            if (Language == newLanguage)
            {
                return;
            }
            OnChangeLanguage(newLanguage);
            Language = newLanguage;
        }
    }
}