
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
#endif

namespace CMSuniVortex.GoogleSheet
{
    public class GoogleSheetCuvClient : CuvClient<GoogleSheetModel, GoogleSheetCuvModelList>, ICuvDoc
    {
        [SerializeField] string _sheetID;
        [SerializeField] string[] _sheetNames;
        [SerializeField] string _jsonKeyPath;

#if UNITY_EDITOR
        ICredential _credential;
        GoogleCredential _googleCredential;
#endif

        public override int GetRepeatCount() => _sheetNames.Length;

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
            return true;
        }

        protected override void OnLoad(int currentRound, string guid, GoogleSheetCuvModelList obj) => obj.SheetName = _sheetNames[currentRound - 1];

        protected override IEnumerator LoadModels(int currentRound, string buildPath, SystemLanguage language, Action<GoogleSheetModel[], string> onSuccess = default, Action<string> onError = default)
        {
#if UNITY_EDITOR
            _credential ??= GoogleSheetUtil.GetCredential(_jsonKeyPath, new[] {SheetsService.Scope.SpreadsheetsReadonly});
            if (_credential == default)
            {
                var error = "Goole auth authentication failed.";
                Debug.LogError(error);
                onError?.Invoke(error);
                yield break;
            }

            var sheetName = _sheetNames[currentRound - 1];
            var task = GoogleSheetUtil.GetSheet(_credential, _sheetID, sheetName);

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
    }
}