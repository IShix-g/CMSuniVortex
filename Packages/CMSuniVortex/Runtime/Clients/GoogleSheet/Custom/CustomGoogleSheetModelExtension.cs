
using System.Collections.Generic;

namespace CMSuniVortex.GoogleSheet
{
    public static class CustomGoogleSheetModelExtension
    {
        public static void FillContentsWithFilteredSheetData(this IList<IList<object>> sheet, Dictionary<string, string> contents, string keyId, int index)
        {
            contents.Clear();
            for (var i = 0; i < sheet[index].Count; i++)
            {
                var id = SafeToString(sheet[0][i]);
                if (string.IsNullOrEmpty(id))
                {
                    continue;
                }
                var value = SafeToString(sheet[index][i]);
                if (id == keyId && string.IsNullOrEmpty(value))
                {
                    continue;
                }
                contents.Add(id, value);
            }
        }
        
        static string SafeToString(object cell) => cell?.ToString()?.Trim() ?? string.Empty;
    }
}