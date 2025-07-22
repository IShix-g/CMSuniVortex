
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using System.Threading;
using UnityEditor;
using Google.Apis.Drive.v3;
using Google.Apis.Sheets.v4;
#endif

namespace CMSuniVortex.GoogleSheet
{
    public class GoogleSheetCuvClient : GoogleSheetCuvClientBase<GoogleSheetModel, GoogleSheetCuvModelList>, ICuvLocalizedClient
    {
        [SerializeField, Tooltip("You can change the Key name that must be set in GoogleSheet.")] string _keyName = "Key";
        [SerializeField] SystemLanguage[] _languages;
        [SerializeField] string[] _sheetNames;
        
#if UNITY_EDITOR
        string _modifiedTime;
        CancellationTokenSource _source;
#endif

        protected override void OnDeselect()
        {
            base.OnDeselect();
#if UNITY_EDITOR
            _source?.SafeCancelAndDispose();
#endif
        }

        public override string GetKeyName() => _keyName;
        
        public override IReadOnlyList<string> GetCuvIds()
            => _languages.Select(language => language.ToString()).ToList();

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

        protected override IEnumerator LoadModels(int currentRound, string buildPath, string cuvId, Action<GoogleSheetModel[], string> onSuccess = default, Action<string> onError = default)
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
            
            _source = new CancellationTokenSource();
            var opSheet = GoogleSheetService.GetSheet(credential, sheetID, sheetName, _source.Token);
            var opModified = GoogleSheetService.GetModifiedTime(credential, sheetID, _source.Token);

            while (!opSheet.IsCompleted
                    || !opModified.IsCompleted)
            {
                yield return default;
            }
            
            if (opSheet.IsCanceled)
            {
                var error = "The operation was canceled.";
                Debug.LogWarning(error);
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
            var keyValue = GetKeyName().Trim();
            var keyIndex = sheet[0].IndexOf(keyValue);
            
            if (keyIndex < 0)
            {
                keyValue = keyValue.ToLower();
                keyIndex = sheet[0].IndexOf(keyValue);
            }
            
            var langIndex = sheet[0].IndexOf(cuvId);
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
                var error = "Language that does not exist : " + cuvId;
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
                onSuccess?.Invoke(models.ToArray(), nameof(GoogleSheetCuvModelList) + "_" + sheetName + "_" + cuvId);
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

        public SystemLanguage[] GetLanguages() => _languages;
    }
}