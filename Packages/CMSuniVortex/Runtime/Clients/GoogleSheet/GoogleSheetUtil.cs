
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;

namespace CMSuniVortex.GoogleSheet
{
    public static class GoogleSheetUtil
    {
        internal static ICredential GetCredential(string keyPath, string[] scopes)
        {
            using var stream = new FileStream(keyPath, FileMode.Open, FileAccess.Read);
            var credential = GoogleCredential.FromStream(stream).CreateScoped(scopes).UnderlyingCredential;
            return credential;
        }

        public static async Task<IList<IList<object>>> GetSheet(ICredential credential, string sheetId, string sheetRange = "Sheet!A:Z")
        {
            var sheetService = new SheetsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "CMSuniVortex Google Sheets Integration"
            });
            var request = sheetService.Spreadsheets.Values.Get(sheetId, sheetRange);
            var response = await request.ExecuteAsync();
            return response.Values;
        }
    }
}