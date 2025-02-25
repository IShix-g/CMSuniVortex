
using UnityEngine;

namespace CMSuniVortex.GoogleSheet
{
    public class GoogleSheetCuvModelList : GoogleSheetCuvModelListBase<GoogleSheetModel>
    {
        public string SheetName;

        public string GetTextByKey(string key)
        {
            if (TryGetByKey(key, out var obj))
            {
                return obj.Text;
            }
#if DEBUG
            Debug.LogError("Key that does not exist: " + key);
#endif
            return string.Empty;
        }
    }
}