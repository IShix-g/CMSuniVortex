
using System;

namespace CMSuniVortex.GoogleSheet
{
    [Serializable]
    public abstract class GoogleSheetModelBase : ICuvModel
    {
        public string Key;
        
        public string GetKey() => Key;
    }
}