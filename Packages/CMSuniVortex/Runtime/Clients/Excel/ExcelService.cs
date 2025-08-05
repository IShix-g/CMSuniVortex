#if UNITY_EDITOR
using System;
using System.Data;
using System.IO;
using ExcelDataReader;

namespace CMSuniVortex.Excel
{
    public sealed class ExcelService 
    {
        public static DataTable GetSheet(string filePath, string sheetName)
        {
            var sheets = GetSheets(filePath);
            for (var i = 0; i < sheets.Tables.Count; i++)
            {
                var table = sheets.Tables[i];
                if (table.TableName == sheetName)
                {
                    return table;
                }
            }
            throw new FileNotFoundException("Sheet not found. sheetName: " + sheetName);
        }
        
        public static DateTimeOffset? GetModifiedTime(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return default;
            }
            var fileInfo = new FileInfo(filePath);
            var lastModifiedUtc = fileInfo.LastWriteTimeUtc;
            return new DateTimeOffset(lastModifiedUtc, TimeSpan.Zero);
        }
        
        public static (bool HasUpdate, string Details) CheckForUpdate(string filePath, DateTime? editorModifiedTime)
        {
            var result = GetModifiedTime(filePath);
            if (string.IsNullOrEmpty(result.ToString()))
            {
                throw new OperationCanceledException("Failed to retrieve data correctly.");
            }
            var sheetTime = DateTime.Parse(result!.ToString());
            var msg = $"Sheet: {sheetTime:MM/dd/yyyy HH:mm}";
            if (editorModifiedTime.HasValue)
            {
                msg += $"\nEditor: {editorModifiedTime.Value:MM/dd/yyyy HH:mm}";
                return (sheetTime > editorModifiedTime.Value, msg);
            }
            msg += "\nEditor: Not Generated Yet";
            return (true, msg);
        }
        
        public static DataSet GetSheets(string filePath)
        {
            using var stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
            using var reader = ExcelReaderFactory.CreateReader(stream);
            return reader.AsDataSet();
        }
    }
}
#endif