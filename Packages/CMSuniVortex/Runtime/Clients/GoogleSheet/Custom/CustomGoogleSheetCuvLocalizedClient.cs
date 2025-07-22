
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CMSuniVortex.GoogleSheet
{
    public abstract class CustomGoogleSheetCuvLocalizedClient<T, TS>
        : CustomGoogleSheetCuvClientBase<T, TS> , ICuvLocalizedClient
        where T : CustomGoogleSheetModel, new() 
        where TS : CustomGoogleSheetCuvModelList<T>
    {
        [SerializeField, Tooltip("You can change the Key name that must be set in GoogleSheet.")] string _keyName = "Key";
        [SerializeField] SystemLanguage[] _languages;
        
        public override string GetKeyName() => _keyName;
        
        public override IReadOnlyList<string> GetCuvIds()
            => _languages.Select(language => language.ToString()).ToList();

        public SystemLanguage[] GetLanguages() => _languages;
    }
}