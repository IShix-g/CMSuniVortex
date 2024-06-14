
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
        [SerializeField] string _sheetID;
        [SerializeField] string _jsonKeyPath;
        
#if UNITY_EDITOR
        ICredential _credential;
        GoogleCredential _googleCredential;
        string _modifiedTime;
#endif

        public void SetSheetID(string sheetID) => _sheetID = sheetID;
        
        public void SetJsonKeyPath(string jsonKeyPath) => _jsonKeyPath = jsonKeyPath;

        protected override void OnLoad(int currentRound, string guid, TS obj)
        {
#if UNITY_EDITOR
            obj.SheetID = _sheetID;
            obj.ModifiedDate = _modifiedTime;
#endif
        }
        
        public override bool CanILoad()
        {
            if (!base.CanILoad())
            {
                return false;
            }
            if (string.IsNullOrEmpty(_sheetID))
            {
                Debug.LogError("Please input the Sheet ID.");
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
            
            var opSheet = GoogleSheetService.GetSheet(_credential, _sheetID, language.ToString());
            var opModified = GoogleSheetService.GetModifiedTime(_credential, _sheetID);

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
            var keyIndex = sheet[0].IndexOf("Key");

            if (keyIndex < 0)
            {
                var error = "Key does not exist.";
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
                
                sheet.FillContentsWithFilteredSheetData(contents, "Key", i);
                
                var model = new T { Key = key };
                var od = (IObjectDeserializer) model;
                od.Deserialize(contents);
                model.SetData(buildPath);
                if (model.ResourcesLoadCoroutines != default)
                {
                    foreach (var enumerator in model.ResourcesLoadCoroutines)
                    {
                        yield return enumerator;
                    }
                }
                od.Deserialized();
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
    }
}