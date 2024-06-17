
using System;
using UnityEngine;

namespace CMSuniVortex
{
    public interface ICuvReference<T, out TS> where T : ICuvModel where TS : ICuvModelList<T>
    {
        event Action<SystemLanguage> OnChangeLanguage;
        SystemLanguage Language { get; }
        void ChangeLanguage(SystemLanguage language);
        TS GetList();
        T GetById(string id);
        bool TryGetById(string id, out T model);
    }
}