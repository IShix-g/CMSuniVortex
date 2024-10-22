
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
    public abstract class CustomGoogleSheetCuvClientBase<T, TS> : CuvClient<T, TS>, ICuvDoc where T : CustomGoogleSheetModel, new() where TS : CustomGoogleSheetCuvModelList<T>
    {
        static readonly Regex s_sheetUrlRegex = new Regex(@"spreadsheets/d/([a-zA-Z0-9-_]+)", RegexOptions.Compiled);
        
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
        string _modifiedTime;
#endif
        
        protected override void OnLoad(int currentRound, string guid, TS obj)
        {
#if UNITY_EDITOR
            obj.SheetID = ExtractSheetIdFromUrl(SheetUrl);
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

        protected IEnumerator GetSheet(string sheetRange, Action<IList<IList<object>>> onSuccess, Action<string> onError = default)
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
            var opSheet = GoogleSheetService.GetSheet(_credential, sheetID, sheetRange);
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
            
            onSuccess?.Invoke(opSheet.Result);
#else
            return default;
#endif
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
        
        public virtual string GetCmsName() => "Google Sheet";

        public virtual string GetDocUrl() => "https://github.com/IShix-g/CMSuniVortex/blob/main/docs/IntegrationWithGoogleSheet.md";
    }
}