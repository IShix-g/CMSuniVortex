
using System.Collections.Generic;

namespace CMSuniVortex.GoogleSheet
{
    public static class CustomGoogleSheetModelExtension
    {
        public static void FillContentsWithFilteredSheetData(this IList<IList<object>> sheet, Dictionary<string, string> contents, string keyId, int index)
        {
            contents.Clear();
            for (var s = 0; s < sheet[index].Count; s++)
            {
                var id = sheet[0][s].ToString();
                if (string.IsNullOrEmpty(id))
                {
                    continue;
                }
                var value = sheet[index][s].ToString();
                if (id == keyId
                    && string.IsNullOrEmpty(value))
                {
                    continue;
                }
                contents.Add(id, value);
            }
        }
    }
}