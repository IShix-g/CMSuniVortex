
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CMSuniVortex.Cockpit
{
    public abstract class CockpitCuvLocalizedClient<T, TS> 
        : CockpitCuvClientBase<T, TS>, ICuvLocalizedClient
        where T : CockpitModel 
        where TS : CockpitCuvModelList<T>
    {
        [SerializeField] SystemLanguage[] _languages;
        
        public override IReadOnlyList<string> GetCuvIds()
            => _languages.Select(language => language.ToString()).ToList();

        public SystemLanguage[] GetLanguages() => _languages;
    }
}