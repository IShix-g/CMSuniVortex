
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CMSuniVortex.GoogleSheet;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace CMSuniVortex.Tests
{
    public class GoogleSheetTest
    {
        const string _jsonKeyPath = "Assets/googleSheetTest/ul-project-388509-142601d4b840.json";
        const string _sheetID = "13XEuxW89jT4ICb2guBcgcgPrCmY_oGxDQgiWNOth7ww";
        const SystemLanguage _language = SystemLanguage.English;

        ICredential _credential;
        
        [SetUp]
        public void SetUp()
        {
            _credential = GoogleSheetUtil.GetCredential(_jsonKeyPath, new[] {SheetsService.Scope.SpreadsheetsReadonly, DriveService.Scope.DriveReadonly});
        }

        [UnityTest]
        public IEnumerator LoadSheetTest()
        {
            if (_credential == default)
            {
                throw new ApplicationException("Google auth authentication failed.");
            }
            
            var op = GoogleSheetUtil.GetSheet(_credential, _sheetID, _language.ToString());
            while (!op.IsCompleted)
            {
                yield return default;
            }
            
            if (op.IsFaulted)
            {
                Debug.LogError("Failed to get sheet: " + op.Exception);
                yield break;
            }
            
            var sheet = op.Result;
            var contents = new Dictionary<string, string>();
            sheet.FillContentsWithFilteredSheetData(contents, "Key", 1);
            Debug.Log(contents.Select(x => "(" + x.Key + " | " + x.Value + ")").Aggregate((a, b) => a + "," + b));
        }
        
        [Test]
        public void SheetUpdateTest()
        {
            if (_credential == default)
            {
                throw new ApplicationException("Google auth authentication failed.");
            }
            
            var driveService = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = _credential,
                ApplicationName = "CMSuniVortex Test",
            });
            
            var request = driveService.Files.Get(_sheetID);
            request.Fields = "modifiedTime";
            var file = request.Execute();

            Debug.Log("Last modified: " + file.ModifiedTimeDateTimeOffset);
        }
    }
}