
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
#if UNITY_EDITOR
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Sheets.v4;
#endif

namespace CMSuniVortex.GoogleSheet
{
    public abstract class CustomGoogleSheetCuvClient<T, TS> : CuvClient<T, TS>, ICuvDoc where T : CustomGoogleSheetModel, new() where TS : CustomGoogleSheetCuvModelList<T>
    {
        [SerializeField, CuvOpenUrl] string _sheetUrl;
        [SerializeField, CuvFilePath("json")] string _jsonKeyPath;
        
        public string SheetUrl
        {
            get => _sheetUrl;
            set => _sheetUrl = value;
        }
        public string JsonKeyPath
        {
            get => _jsonKeyPath;
            set => _jsonKeyPath = value;
        }
        
#if UNITY_EDITOR
        ICredential _credential;
        GoogleCredential _googleCredential;
        string _modifiedTime;
#endif
        
        protected override void OnLoad(int currentRound, string guid, TS obj)
        {
#if UNITY_EDITOR
            obj.SheetID = ExtractSheetIdFromUrl(_sheetUrl);
            obj.ModifiedDate = _modifiedTime;
#endif
        }
        
        public override bool CanILoad()
        {
            if (!base.CanILoad())
            {
                return false;
            }
            if (string.IsNullOrEmpty(_sheetUrl))
            {
                Debug.LogError("Please input the Sheet Url.");
                return false;
            }
            if (string.IsNullOrEmpty(_jsonKeyPath))
            {
                Debug.LogError("Please input the Json Key Path.");
                return false;
            }
            if (!File.Exists(_jsonKeyPath))
            {
                Debug.LogError("Json Key Path does not exist.");
                return false;
            }
            if (!IsSheetUrlValid(_sheetUrl))
            {
                Debug.LogError("Could not convert Sheet Url to Sheet Id, please check if the URL is correct.");
                return false;
            }
            return true;
        }
        
        protected override IEnumerator LoadModels(int currentRound, string buildPath, SystemLanguage language, Action<T[], string> onSuccess = default, Action<string> onError = default)
        {
#if UNITY_EDITOR
            _credential ??= GoogleSheetService.GetCredential(_jsonKeyPath, new[] { SheetsService.Scope.SpreadsheetsReadonly, DriveService.Scope.DriveReadonly });
            if (_credential == default)
            {
                var error = "Google auth authentication failed.";
                Debug.LogError(error);
                onError?.Invoke(error);
                yield break;
            }

            var sheetID = ExtractSheetIdFromUrl(_sheetUrl);
            var opSheet = GoogleSheetService.GetSheet(_credential, sheetID, language.ToString());
            var opModified = GoogleSheetService.GetModifiedTime(_credential, sheetID);

            while (!opSheet.IsCompleted
                   || !opModified.IsCompleted)
            {
                yield return default;
            }
            if (opSheet.IsCanceled)
            {
                var error = "The operation was canceled.";
                Debug.LogError(error);
                onError?.Invoke(error);
                yield break;
            }
            if (opSheet.IsFaulted)
            {
                var error = "Failed to get sheet: " + opSheet.Exception;
                Debug.LogError(error);
                onError?.Invoke(error);
                yield break;
            }
            
            _modifiedTime = !opModified.IsFaulted
                ? opModified.Result?.ToString()
                : string.Empty;
            
            var sheet = opSheet.Result;
            var keyValue = "Key";
            var keyIndex = sheet[0].IndexOf(keyValue);

            if (keyIndex < 0)
            {
                keyValue = "key";
                keyIndex = sheet[0].IndexOf(keyValue);
            }
            
            if (keyIndex < 0)
            {
                var error = "Could not find the key field. Please be sure to set it. For more information, click here https://github.com/IShix-g/CMSuniVortex/blob/main/docs/IntegrationWithGoogleSheet.md";
                Debug.LogError(error);
                onError?.Invoke(error);
                yield break;
            }

            var contents = new Dictionary<string, string>();
            var models = new List<T>();
            for (var i = 1; i < sheet.Count; i++)
            {
                var key = sheet[i][keyIndex].ToString();
                if (string.IsNullOrEmpty(key))
                {
                    continue;
                }
                
                sheet.FillContentsWithFilteredSheetData(contents, keyValue, i);
                
                var model = new T { Key = key };
                ((IObjectDeserializer) model).Deserialize(contents);
                model.SetData(buildPath);
                if (model.ResourcesLoadCoroutines != default)
                {
                    foreach (var enumerator in model.ResourcesLoadCoroutines)
                    {
                        yield return enumerator;
                    }
                }
                models.Add(model);
            }

            if (models.Count > 0)
            {
                onSuccess?.Invoke(models.ToArray(), typeof(TS).Name + "_" + language);
            }
            else
            {
                var error = "There was no content to display.";
                Debug.LogError(error);
                onError?.Invoke(error);
            }
#else
            return default;
#endif
        }
        
        string ICuvDoc.GetCmsName() => "Google Sheet";

        string ICuvDoc.GetDocUrl() => "https://github.com/IShix-g/CMSuniVortex/blob/main/docs/IntegrationWithGoogleSheet.md";
        
        public static bool IsSheetUrlValid(string sheetUrl)
        {
            var regex = new Regex(@"spreadsheets/d/([a-zA-Z0-9-_]+)");
            var match = regex.Match(sheetUrl);
            return match.Success;
        }

        public static string ExtractSheetIdFromUrl(string sheetUrl)
        {
            var regex = new Regex(@"spreadsheets/d/([a-zA-Z0-9-_]+)");
            var match = regex.Match(sheetUrl);
            if (match.Success)
            {
                return match.Groups[1].Value;
            }
            throw new ArgumentException("Could not convert Sheet Url to Sheet Id, please check if the URL is correct.");
        }
    }
}