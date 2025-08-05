
using System.Collections.Generic;
using UnityEngine;

namespace CMSuniVortex.Excel
{
    public abstract class ExcelCuvClient<T, TS> : ExcelCuvClientBase<T, TS> where T : ExcelModel, new() where TS : ExcelCuvModelList<T>
    {
        [SerializeField] string[] _sheetNames;

        public override IReadOnlyList<string> GetCuvIds() => _sheetNames;
    }
}