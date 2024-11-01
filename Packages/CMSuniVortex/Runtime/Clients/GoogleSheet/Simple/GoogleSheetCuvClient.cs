
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using Google.Apis.Drive.v3;
using Google.Apis.Sheets.v4;
#endif

namespace CMSuniVortex.GoogleSheet
{
    public class GoogleSheetCuvClient : CuvClient<GoogleSheetModel, GoogleSheetCuvModelList>, ICuvDoc, ICuvUpdateChecker
    {
        [SerializeField, CuvOpenUrl] string _sheetUrl;
        [SerializeField] string[] _sheetNames;
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
        string _modifiedTime;
#endif

        public override int GetRepeatCount() => _sheetNames.Length;
        
        public override bool CanILoad()
        {
            if (!base.CanILoad())
            {
                return false;
            }
            if (string.IsNullOrEmpty(_sheetUrl))
            {
                Debug.LogError("Please input the Sheet ID.");
                return false;
            }
            if (_sheetNames.Length == 0
                || _sheetNames.Any(string.IsNullOrEmpty))
            {
                Debug.LogError("Please input the Sheet Name.");
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
            if (!GoogleSheetService.IsSheetUrlValid(_sheetUrl))
            {
                Debug.LogError("Could not convert Sheet Url to Sheet Id, please check if the URL is correct.");
                return false;
            }
            return true;
        }

        protected override void OnLoad(int currentRound, string guid, GoogleSheetCuvModelList obj)
        {
#if UNITY_EDITOR
            obj.SheetName = _sheetNames[currentRound - 1];
            obj.ModifiedDate = _modifiedTime;
#endif
        }

        protected override IEnumerator LoadModels(int currentRound, string buildPath, SystemLanguage language, Action<GoogleSheetModel[], string> onSuccess = default, Action<string> onError = default)
        {
#if UNITY_EDITOR
            var credential = GoogleSheetService.GetCredential(_jsonKeyPath, new[] { SheetsService.Scope.SpreadsheetsReadonly, DriveService.Scope.DriveReadonly });

            if (credential == default)
            {
                var error = "Goole auth authentication failed.";
                Debug.LogError(error);
                onError?.Invoke(error);
                yield break;
            }
            
            var sheetID = GoogleSheetService.ExtractSheetIdFromUrl(_sheetUrl);
            var sheetName = _sheetNames[currentRound - 1];
            var opSheet = GoogleSheetService.GetSheet(credential, sheetID, sheetName);
            var opModified = GoogleSheetService.GetModifiedTime(credential, sheetID);

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
            
            var langIndex = sheet[0].IndexOf(language.ToString());
            var commentIndex = sheet[0].IndexOf("Comment");

            if (keyIndex < 0)
            {
                var error = "Key does not exist.";
                Debug.LogError(error);
                onError?.Invoke(error);
                yield break;
            }
            if (langIndex < 0)
            {
                var error = "Language that does not exist : " + language;
                Debug.LogError(error);
                onError?.Invoke(error);
                yield break;
            }
            
            var models = new List<GoogleSheetModel>();
            for (var i = 1; i < sheet.Count; i++)
            {
                var length = sheet[i].Count;
                if (length == 0 || keyIndex >= length)
                {
                    continue;
                }
                var key = sheet[i][keyIndex].ToString();
                var text = sheet[i][langIndex].ToString();
                if (string.IsNullOrEmpty(key)
                    || string.IsNullOrEmpty(text))
                {
                    continue;
                }

                var model = new GoogleSheetModel
                {
                    Key = key,
                    Text = text,
                    Comment = sheet[i][commentIndex].ToString()
                };
                models.Add(model);
            }

            if (models.Count > 0)
            {
                onSuccess?.Invoke(models.ToArray(), nameof(GoogleSheetCuvModelList) + "_" + sheetName + "_" + language);
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
        
        
        bool ICuvUpdateChecker.IsUpdateAvailable()
            => !string.IsNullOrEmpty(_sheetUrl)
               && _sheetNames is {Length: > 0} 
               && !string.IsNullOrEmpty(_jsonKeyPath);

        void ICuvUpdateChecker.CheckForUpdate(string buildPath, Action<bool, string> successAction, Action<string> failureAction)
        {
#if UNITY_EDITOR
            var credential = GoogleSheetService.GetCredential(_jsonKeyPath, new[] { DriveService.Scope.DriveReadonly });
            var sheetID = GoogleSheetService.ExtractSheetIdFromUrl(_sheetUrl);
            
            GoogleSheetService.GetModifiedTime(credential, sheetID)
                .SafeContinueWith(task =>
                {
                    if (task.IsFaulted)
                    {
                        failureAction?.Invoke("Failed to retrieve updates from sheets.");
                        if (task.Exception != default)
                        {
                            throw task.Exception;
                        }
                        return;
                    }
                    
                    var result = task.Result?.ToString();
                    if (string.IsNullOrEmpty(result))
                    {
                        failureAction?.Invoke("Failed to retrieve data correctly.");
                        if (task.Exception != default)
                        {
                            throw task.Exception;
                        }
                        return;
                    }
                
                    var hasUpdate = default(bool);
                    var sheetTime = DateTime.Parse(result);
                    var msg = "Sheet: " + sheetTime.ToString("MM/dd/yyyy HH:mm");
                    var editorTime = GetModifiedTimeFromEditor(buildPath);
                    if (editorTime.HasValue)
                    {
                        msg += "\nEditor: " + editorTime.Value.ToString("MM/dd/yyyy HH:mm");
                        hasUpdate = sheetTime > editorTime.Value;
                    }
                    else
                    {
                        msg += "\nEditor: N/A";
                        hasUpdate = true;
                    }
                    successAction?.Invoke(hasUpdate, msg);
                });
#endif
        }

#if UNITY_EDITOR
        DateTime? GetModifiedTimeFromEditor(string buildPath)
        {
            if (string.IsNullOrEmpty(_sheetUrl))
            {
                return null;
            }
            if (Path.HasExtension(buildPath))
            {
                buildPath = Path.GetDirectoryName(buildPath);
            }
            
            var assets = AssetDatabase.FindAssets("t:" + typeof(GoogleSheetCuvModelList), new []{ buildPath })
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<GoogleSheetCuvModelList>)
                .ToArray();

            var latestAsset = assets.OrderByDescending(asset => asset.ModifiedDate)
                .FirstOrDefault();
            if (latestAsset != default
                && !string.IsNullOrEmpty(latestAsset.ModifiedDate))
            {
                return DateTime.Parse(latestAsset.ModifiedDate);
            }
            return default;
        }
#endif
    }
}