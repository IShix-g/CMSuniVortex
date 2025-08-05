
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CMSuniVortex.Excel
{
    public abstract class ExcelCuvLocalizedClient<T, TS>
        : ExcelCuvClientBase<T, TS>, ICuvLocalizedClient
        where T : ExcelModel, new()
        where TS : ExcelCuvModelList<T>
    {
        [SerializeField] SystemLanguage[] _languages;
        
        public override IReadOnlyList<string> GetCuvIds()
            => _languages.Select(language => language.ToString()).ToList();

        public SystemLanguage[] GetLanguages() => _languages;
    }
}