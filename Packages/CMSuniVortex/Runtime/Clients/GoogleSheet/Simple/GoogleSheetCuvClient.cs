
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using Google.Apis.Drive.v3;
using Google.Apis.Sheets.v4;
#endif

namespace CMSuniVortex.GoogleSheet
{
    public class GoogleSheetCuvClient : GoogleSheetCuvClientBase<GoogleSheetModel, GoogleSheetCuvModelList>
    {
        [SerializeField] string[] _sheetNames;
        
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
            if (_sheetNames.Length == 0
                || _sheetNames.Any(string.IsNullOrEmpty))
            {
                Debug.LogError("Please input the Sheet Name.");
                return false;
            }
            return true;
        }

        protected override void OnLoad(int currentRound, string guid, GoogleSheetCuvModelList obj)
        {
            base.OnLoad(currentRound, guid, obj);
#if UNITY_EDITOR
            obj.SheetName = _sheetNames[currentRound - 1];
            obj.ModifiedDate = _modifiedTime;
#endif
        }

        protected override IEnumerator LoadModels(int currentRound, string buildPath, SystemLanguage language, Action<GoogleSheetModel[], string> onSuccess = default, Action<string> onError = default)
        {
#if UNITY_EDITOR
            var credential = GoogleSheetService.GetCredential(JsonKeyPath, new[] { SheetsService.Scope.SpreadsheetsReadonly, DriveService.Scope.DriveReadonly });

            if (credential == default)
            {
                var error = "Google auth authentication failed.";
                Debug.LogError(error);
                onError?.Invoke(error);
                yield break;
            }
            
            var sheetID = GoogleSheetService.ExtractSheetIdFromUrl(SheetUrl);
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
        
        public override bool IsUpdateAvailable()
            => base.IsUpdateAvailable()
               && _sheetNames is {Length: > 0};
        
        protected override GoogleSheetCuvModelList[] LoadEditorCuvModelLists(string buildPath)
        {
#if UNITY_EDITOR
            return AssetDatabase.FindAssets("t:" + typeof(GoogleSheetCuvModelList), new []{ buildPath })
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<GoogleSheetCuvModelList>)
                .ToArray();
#else
            return default;
#endif
        }
    }
}