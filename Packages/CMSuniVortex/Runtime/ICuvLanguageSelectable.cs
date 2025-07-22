
using System.Collections.Generic;
using UnityEngine;

namespace CMSuniVortex
{
    public interface ICuvLanguageSelectable
    {
        SystemLanguage ActiveLanguage { get; }
        IReadOnlyList<SystemLanguage> GetLanguages();
        void ChangeLanguage(SystemLanguage language);
    }
}