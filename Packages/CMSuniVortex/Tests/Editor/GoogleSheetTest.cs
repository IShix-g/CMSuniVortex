
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
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
using Debug = UnityEngine.Debug;

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
            _credential = GoogleSheetService.GetCredential(_jsonKeyPath, new[] {SheetsService.Scope.SpreadsheetsReadonly, DriveService.Scope.DriveReadonly});
        }

        [UnityTest]
        public IEnumerator LoadSheetTest()
        {
            if (_credential == default)
            {
                throw new ApplicationException("Google auth authentication failed.");
            }
            
            var source = new CancellationTokenSource();
            var op = GoogleSheetService.GetSheet(_credential, _sheetID, _language.ToString(), source.Token);
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
        
        class TestModel : CustomGoogleSheetModel
        {
            IReadOnlyDictionary<string, TestEnum> _maps;
            
            public enum TestEnum { Enum1, Enum2 }
            
            public void SetMap(IReadOnlyDictionary<string, TestEnum> maps) => _maps = maps;
            
            protected override void OnDeserialize()
            {
                {
                    var result = TryGetEnum<TestEnum>("A", _maps, out var value);
                    Assert.That(result, Is.EqualTo(CuvEnumResult.Success));
                    Assert.That(value, Is.EqualTo(TestEnum.Enum1));
                }
                {
                    var result = TryGetEnum<TestEnum>("B", _maps, out var value);
                    Assert.That(result, Is.EqualTo(CuvEnumResult.EmptyValue));
                    Assert.That(value, Is.EqualTo(TestEnum.Enum1));
                }
                {
                    var result = TryGetEnum<TestEnum>("C", _maps, out var value);
                    Assert.That(result, Is.EqualTo(CuvEnumResult.NotFoundInMap));
                    Assert.That(value, Is.EqualTo(TestEnum.Enum1));
                }
                {
                    var result = TryGetEnum<TestEnum>("Z", _maps, out var value);
                    Assert.That(result, Is.EqualTo(CuvEnumResult.NotHasKey));
                    Assert.That(value, Is.EqualTo(TestEnum.Enum1));
                }
            }
        }
        
        [Test]
        public void EnumDeserializeTest()
        {
            var model = new TestModel();
            model.SetMap(new Dictionary<string, TestModel.TestEnum>()
            {
                {"AA", TestModel.TestEnum.Enum1},
                {"BB", TestModel.TestEnum.Enum2},
            });
            
            ((IObjectDeserializer) model).Deserialize(new Dictionary<string, string>()
            {
                {"A", "AA"},
                {"B", ""},
                {"C", "CC"},
            });
        }
    }
}