
using System;
using UnityEngine;

namespace CMSuniVortex
{
    /// <summary>
    /// Managing a reference to a list of models.
    /// </summary>
    public abstract class CuvReference<T, TS> : ScriptableObject, ICuvReference<T, TS> where T : ICuvModel where TS : ICuvModelList<T>
    {
        public event Action<SystemLanguage> OnChangeLanguage = delegate {};
        
        [SerializeField] TS[] _modelLists;
        
        public SystemLanguage Language { get; private set; }
        public TS[] ModelLists => _modelLists;
        
        int _currentLanguageIndex;

        void OnEnable()
        {
            if (_modelLists is {Length: > 0})
            {
                ChangeLanguage(Application.systemLanguage);
            }
        }

        public void SetModelLists(TS[] modelLists) => _modelLists = modelLists;

        public TS GetList() => _modelLists[_currentLanguageIndex];

        public T GetById(string id)
        {
            if (GetList().TryGetById(id, out var model))
            {
                return model;
            }
            throw new ArgumentException("Id that does not exist : " + id);
        }
        
        public bool TryGetById(string id, out T model)
        {
            model = GetList().GetById(id);
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