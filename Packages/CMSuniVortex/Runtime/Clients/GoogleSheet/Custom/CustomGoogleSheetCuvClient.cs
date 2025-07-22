
using System.Collections.Generic;
using UnityEngine;

namespace CMSuniVortex.GoogleSheet
{
    public abstract class CustomGoogleSheetCuvClient<T, TS>
        : CustomGoogleSheetCuvClientBase<T, TS> 
        where T : CustomGoogleSheetModel, new() 
        where TS : CustomGoogleSheetCuvModelList<T>
    {
        [SerializeField, Tooltip("You can change the Key name that must be set in GoogleSheet.")] string _keyName = "Key";
        [SerializeField] string[] _sheetNames;

        public override IReadOnlyList<string> GetCuvIds() => _sheetNames;
        
        public override string GetKeyName() => _keyName;
    }
}