
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
    public abstract class CustomGoogleSheetCuvClientBase<T, TS> : GoogleSheetCuvClientBase<T, TS> where T : CustomGoogleSheetModel, new() where TS : CustomGoogleSheetCuvModelList<T>
    {
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

        protected IEnumerator GetSheet(string sheetRange, Action<IList<IList<object>>> onSuccess, Action<string> onError = default)
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
            
            _source = new CancellationTokenSource();
            var opSheet = GoogleSheetService.GetSheet(credential, sheetID, sheetRange, _source.Token);
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
            
            _source?.Dispose();
            _source = default;
            
            onSuccess?.Invoke(opSheet.Result);
#else
            return default;
#endif
        }
        
        protected override IEnumerator LoadModels(int currentRound, string buildPath, string cuvId, Action<T[], string> onSuccess = default, Action<string> onError = default)
        {
#if UNITY_EDITOR
            var sheet = default(IList<IList<object>>);
            yield return GetSheet(cuvId, results => sheet = results, onError);

            if (sheet == null
                || sheet.Count == 0)
            {
                var error = "No data existed on the sheet. url: " + SheetUrl;
                Debug.LogError(error);
                onError?.Invoke(error);
                yield break;
            }
            
            var keyValue = GetKeyName().Trim();
            var keyIndex = sheet[0].IndexOf(keyValue);

            if (keyIndex < 0)
            {
                keyValue = keyValue.ToLower();
                keyIndex = sheet[0].IndexOf(keyValue);
            }
            
            if (keyIndex < 0)
            {
                var error = "Could not find the key field. Please be sure to set it. For more information, click here " + GetDocUrl();
                Debug.LogError(error);
                onError?.Invoke(error);
                yield break;
            }

            var contents = new Dictionary<string, string>();
            var models = new List<T>();
            for (var i = 1; i < sheet.Count; i++)
            {
                var length = sheet[i].Count;
                if (length == 0 || keyIndex >= length)
                {
                    continue;
                }
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
                onSuccess?.Invoke(models.ToArray(), typeof(TS).Name + "_" + cuvId);
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
        
        protected override TS[] LoadEditorCuvModelLists(string buildPath)
        {
#if UNITY_EDITOR
            return AssetDatabase.FindAssets("t:" + typeof(TS), new []{ buildPath })
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<TS>)
                .ToArray();
#else
            return default;
#endif
        }
    }
}