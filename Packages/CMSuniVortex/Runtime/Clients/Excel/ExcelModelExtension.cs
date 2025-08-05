
using System.Collections.Generic;
using System.Data;

namespace CMSuniVortex.Excel
{
    public static class ExcelModelExtension
    {
        public static void FillContentsWithFilteredSheetData(this DataTable sheet, Dictionary<string, string> contents, string keyId, int index)
        {
            contents.Clear();
            for (var i = 0; i < sheet.Rows[index].ItemArray.Length; i++)
            {
                var id = SafeToString(sheet.Rows[0][i]);
                if (string.IsNullOrEmpty(id))
                {
                    continue;
                }
                var value = SafeToString(sheet.Rows[index][i]);
                if (id == keyId && string.IsNullOrEmpty(value))
                {
                    continue;
                }
                contents.Add(id, value);
            }
        }
        
        public static int IndexOf(this object[] objs, string keyValue)
        {
            for (var i = 0; i < objs.Length; i++)
            {
                var key = SafeToString(objs[i]).Trim().ToLower();
                if (string.IsNullOrEmpty(key)
                    || keyValue != key)
                {
                    continue;
                }

                return i;
            }
            return -1;
        }
        
        static string SafeToString(this object cell) => cell?.ToString()?.Trim() ?? string.Empty;
    }
}