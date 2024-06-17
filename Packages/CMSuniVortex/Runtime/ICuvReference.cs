
using System;
using UnityEngine;

namespace CMSuniVortex
{
    /// <summary>
    /// Managing a reference to a list of models.
    /// </summary>
    public interface ICuvReference
    {
        event Action<SystemLanguage> OnChangeLanguage;
        SystemLanguage Language { get; }
        void ChangeLanguage(SystemLanguage language);
    }
}