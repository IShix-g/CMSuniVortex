using UnityEngine;

namespace CMSuniVortex
{
    public interface ICuvModelListSetter<in T> where T : ICuvModel
    {
        void SetData(SystemLanguage language, T[] models);
    }
}