
using UnityEngine;

namespace CMSuniVortex.GoogleSheet
{
    public class GoogleSheetCuvModelList : CuvModelList<GoogleSheetModel>
    {
        public string SheetName;

        public string GetTextByKey(string key)
        {
            if (TryGetById(key, out var obj))
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