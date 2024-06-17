
using System;
using System.Collections;
using UnityEngine;

namespace CMSuniVortex
{
    public interface ICuvReferenceAsync<T, out TS> where T : ICuvModel where TS : ICuvModelList<T>
    {
        event Action<SystemLanguage> OnChangeLanguage;
        SystemLanguage Language { get; }
        void ChangeLanguage(SystemLanguage language);
        IEnumerator GetList(Action<TS> onLoadedAction);
        IEnumerator GetById(string id, Action<T> onLoadedAction);
        bool TryGetById(string id, out T model);
    }
}