
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using Google.Apis.Auth.OAuth2;
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
#endif
        
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
            _credential ??= GoogleSheetUtil.GetCredential(_jsonKeyPath, new[] {SheetsService.Scope.SpreadsheetsReadonly});
            if (_credential == default)
            {
                var error = "Google auth authentication failed.";
                Debug.LogError(error);
                onError?.Invoke(error);
                yield break;
            }
            
            var task = GoogleSheetUtil.GetSheet(_credential, _sheetID, language.ToString());

            while (!task.IsCompleted)
            {
                yield return default;
            }

            if (task.IsFaulted)
            {
                var error = "Failed to get sheet: " + task.Exception;
                Debug.LogError(error);
                onError?.Invoke(error);
                yield break;
            }
            
            var sheet = task.Result;
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
                contents.Clear();
                for (var s = 0; s < sheet[i].Count; s++)
                {
                    var id = sheet[0][s].ToString();
                    if (string.IsNullOrEmpty(id))
                    {
                        continue;
                    }
                    var value = sheet[i][s].ToString();
                    if (id == "Key"
                        && string.IsNullOrEmpty(value))
                    {
                        continue;
                    }
                    contents.Add(id, value);
                }
                var model = new T { Key = key };
                model.Deserialize(contents);
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
    }
}