#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;

namespace CMSuniVortex.GoogleSheet
{
    public static class GoogleSheetService
    {
        static readonly Regex s_sheetUrlRegex = new Regex(@"spreadsheets/d/([a-zA-Z0-9-_]+)", RegexOptions.Compiled);
        
        public static ICredential GetCredential(string keyPath, string[] scopes)
        {
            using var stream = new FileStream(keyPath, FileMode.Open, FileAccess.Read);
            var credential = GoogleCredential.FromStream(stream).CreateScoped(scopes).UnderlyingCredential;
            return credential;
        }

        public static async Task<IList<IList<object>>> GetSheet(ICredential credential, string sheetId, string sheetRange, CancellationToken token)
        {
            var sheetService = new SheetsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "CMSuniVortex Google Sheets Integration"
            });
            var request = sheetService.Spreadsheets.Values.Get(sheetId, sheetRange);
            var response = await request.ExecuteAsync(token);
            return response.Values;
        }
        
        public static async Task<DateTimeOffset?> GetModifiedTime(ICredential credential, string sheetId, CancellationToken token)
        {
            var driveService = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "CMSuniVortex Google Drive Integration",
            });
            var request = driveService.Files.Get(sheetId);
            request.Fields = "modifiedTime";
            var file = await request.ExecuteAsync(token);
            return file.ModifiedTimeDateTimeOffset;
        }
        
        public static async Task<(bool HasUpdate, string Details)> CheckForUpdate(string sheetUrl, string jsonKeyPath, DateTime? editorLatestModifiedTime, CancellationToken token)
        {
            var credential = GetCredential(jsonKeyPath, new[] { DriveService.Scope.DriveReadonly });
            var sheetID = ExtractSheetIdFromUrl(sheetUrl);
            var result = await GetModifiedTime(credential, sheetID, token);
            if (string.IsNullOrEmpty(result?.ToString()))
            {
                throw new OperationCanceledException("Failed to retrieve data correctly.");
            }
            var sheetTime = DateTime.Parse(result!.ToString());
            var msg = $"Sheet: {sheetTime:MM/dd/yyyy HH:mm}";
            if (editorLatestModifiedTime.HasValue)
            {
                msg += $"\nEditor: {editorLatestModifiedTime.Value:MM/dd/yyyy HH:mm}";
                return (sheetTime > editorLatestModifiedTime.Value, msg);
            }

            msg += "\nEditor: Not Generated Yet";
            return (true, msg);
        }
        
        public static bool IsSheetUrlValid(string sheetUrl) => s_sheetUrlRegex.IsMatch(sheetUrl);

        public static string ExtractSheetIdFromUrl(string sheetUrl)
        {
            var match = s_sheetUrlRegex.Match(sheetUrl);
            if (match.Success)
            {
                return match.Groups[1].Value;
            }
            throw new ArgumentException("Could not convert Sheet Url to Sheet Id, please check if the URL is correct.");
        }
    }
}
#endif