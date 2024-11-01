
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using Google.Apis.Drive.v3;
using Google.Apis.Sheets.v4;
#endif

namespace CMSuniVortex.GoogleSheet
{
    public abstract class CustomGoogleSheetCuvClientBase<T, TS> : CuvClient<T, TS>, ICuvDoc, ICuvUpdateChecker where T : CustomGoogleSheetModel, new() where TS : CustomGoogleSheetCuvModelList<T>
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
        
        protected override void OnLoad(int currentRound, string guid, TS obj)
        {
            base.OnLoad(currentRound, guid, obj);
#if UNITY_EDITOR
            obj.SheetID = GoogleSheetService.ExtractSheetIdFromUrl(SheetUrl);
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
            if (!GoogleSheetService.IsSheetUrlValid(_sheetUrl))
            {
                Debug.LogError("Could not convert Sheet Url to Sheet Id, please check if the URL is correct.");
                return false;
            }
            return true;
        }

        protected IEnumerator GetSheet(string sheetRange, Action<IList<IList<object>>> onSuccess, Action<string> onError = default)
        {
#if UNITY_EDITOR
            var credential = GoogleSheetService.GetCredential(_jsonKeyPath, new[] { SheetsService.Scope.SpreadsheetsReadonly, DriveService.Scope.DriveReadonly });
            if (credential == default)
            {
                var error = "Google auth authentication failed.";
                Debug.LogError(error);
                onError?.Invoke(error);
                yield break;
            }

            var sheetID = GoogleSheetService.ExtractSheetIdFromUrl(_sheetUrl);
            var opSheet = GoogleSheetService.GetSheet(credential, sheetID, sheetRange);
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
            
            onSuccess?.Invoke(opSheet.Result);
#else
            return default;
#endif
        }
        
        public virtual string GetCmsName() => "Google Sheet";

        public virtual string GetDocUrl() => "https://github.com/IShix-g/CMSuniVortex/blob/main/docs/IntegrationWithGoogleSheet.md";
        
        
        bool ICuvUpdateChecker.IsUpdateAvailable()
            => !string.IsNullOrEmpty(_sheetUrl)
               && !string.IsNullOrEmpty(_jsonKeyPath);

        void ICuvUpdateChecker.CheckForUpdate(string buildPath, Action<bool, string> successAction, Action<string> failureAction)
        {
#if UNITY_EDITOR
            _source = new CancellationTokenSource();
            var modifiedTime = GetModifiedTimeFromEditor(_sheetUrl, buildPath);
            GoogleSheetService.CheckForUpdate(_sheetUrl, _jsonKeyPath, modifiedTime)
                .SafeContinueWith(
                task =>
                {
                    try
                    {
                        if (!task.IsFaulted)
                        {
                            var result = task.Result;
                            successAction?.Invoke(result.HasUpdate, result.Details);
                        }
                        else if (task.Exception != default)
                        {
                            failureAction?.Invoke("See console for error details.");
                            throw task.Exception;
                        }
                        else
                        {
                            failureAction?.Invoke("Unknown error. Please try again later.");
                        }
                    }
                    finally
                    {
                        _source.SafeCancelAndDispose();
                        _source = null;
                    }
                }, _source.Token);
#endif
        }
        
        DateTime? GetModifiedTimeFromEditor(string sheetUrl, string buildPath)
        {
#if UNITY_EDITOR
            if (string.IsNullOrEmpty(sheetUrl))
            {
                return default;
            }
            if (Path.HasExtension(buildPath))
            {
                buildPath = Path.GetDirectoryName(buildPath);
            }
            var assets = AssetDatabase.FindAssets("t:" + typeof(TS), new []{ buildPath })
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<TS>)
                .ToArray();
            var latestAsset = assets.OrderByDescending(asset => asset.ModifiedDate).FirstOrDefault();
            if (latestAsset != default
                && !string.IsNullOrEmpty(latestAsset.ModifiedDate))
            {
                return DateTime.Parse(latestAsset.ModifiedDate);
            }
#endif
            return default;
        }
    }
}