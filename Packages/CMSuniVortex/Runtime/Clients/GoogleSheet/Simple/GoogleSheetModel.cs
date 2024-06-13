
using System;

namespace CMSuniVortex.GoogleSheet
{
    [Serializable]
    public class GoogleSheetModel : ICuvModel
    {
        public string Key;
        public string Text;
        public string Comment;
        
        public string GetID() => Key;
    }
}