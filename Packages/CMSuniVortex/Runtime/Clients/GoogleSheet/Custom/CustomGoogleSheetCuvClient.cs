
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CMSuniVortex.GoogleSheet
{
    [CuvClient("Google Sheet")]
    public abstract class CustomGoogleSheetCuvClient<T, TS> : CustomGoogleSheetCuvClientBase<T, TS> where T : CustomGoogleSheetModel, new() where TS : CustomGoogleSheetCuvModelList<T>
    {
        protected override IEnumerator LoadModels(int currentRound, string buildPath, SystemLanguage language, Action<T[], string> onSuccess = default, Action<string> onError = default)
        {
#if UNITY_EDITOR
            var sheet = default(IList<IList<object>>);
            yield return GetSheet(language.ToString(), results => sheet = results, onError);

            if (sheet == null
                || sheet.Count == 0)
            {
                var error = "No data existed on the sheet. url: " + SheetUrl;
                Debug.LogError(error);
                onError?.Invoke(error);
                yield break;
            }
            
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
    }
}