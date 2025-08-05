
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CMSuniVortex.Excel
{
    [CuvClient("Excel")]
    public abstract class ExcelCuvClientBase<T, TS>
        : CuvClient<T, TS>, ICuvDoc, ICuvUpdateChecker 
        where T : ExcelModel, new() 
        where TS : ExcelCuvModelList<T>
    {
        [SerializeField, CuvFilePath("xlsb,xlsx,xls")] string _filePath;
        [SerializeField, Tooltip("You can change the Key name that must be set in Sheet.")] string _keyName = "Key";
        
        public string FilePath
        {
            get => _filePath;
            set => _filePath = value;
        }
        
        public override string GetKeyName() => _keyName;
        
        public override bool CanILoad()
        {
            if (!base.CanILoad())
            {
                return false;
            }
            if (GetCuvIds() == null
                || GetCuvIds().Count == 0)
            {
                Debug.LogError("Please configure at least one language.");
                return false;
            }
            if (string.IsNullOrEmpty(_filePath))
            {
                Debug.LogError("Please input the FilePath.");
                return false;
            }
            return true;
        }
        
        protected override void OnLoad(int currentRound, string guid, TS obj)
        {
            base.OnLoad(currentRound, guid, obj);
#if UNITY_EDITOR
            var dateString = ExcelService.GetModifiedTime(_filePath)?.ToString() ?? string.Empty;
            obj.ModifiedDate = dateString;
#endif
        }

        bool ICuvUpdateChecker.IsUpdateAvailable()
            => !string.IsNullOrEmpty(_filePath);

        void ICuvUpdateChecker.CheckForUpdate(string buildPath, Action<bool, string> successAction, Action<string> failureAction)
        {
#if UNITY_EDITOR
            var lists = LoadEditorCuvModelLists(buildPath);
            var modifiedTime = GetModifiedTimeFromEditor(_filePath, lists);
            var result = ExcelService.CheckForUpdate(_filePath, modifiedTime);
            successAction?.Invoke(result.HasUpdate, result.Details);
#endif
        }
        
        protected override IEnumerator LoadModels(int currentRound, string buildPath, string cuvId, Action<T[], string> onSuccess = default, Action<string> onError = default)
        {
#if UNITY_EDITOR
            var table = ExcelService.GetSheet(_filePath, cuvId);

            if (table == null
                || table.Rows[0].ItemArray.Length == 0)
            {
                var error = "No data existed on the sheet. path: " + _filePath;
                Debug.LogError(error);
                onError?.Invoke(error);
                yield break;
            }
            
            var keyValue = GetKeyName().Trim().ToLower();
            var keyIndex = table.Rows[0].ItemArray.IndexOf(keyValue);
            
            if (keyIndex < 0)
            {
                var error = "Could not find the key field. Please be sure to set it. For more information, click here " + GetDocUrl();
                Debug.LogError(error);
                onError?.Invoke(error);
                yield break;
            }

            var contents = new Dictionary<string, string>();
            var models = new List<T>();
            for (var i = 1; i < table.Rows.Count; i++)
            {
                var obj = table.Rows[i][keyIndex];
                if (obj == default)
                {
                    continue;
                }
                var key = obj.ToString().Trim().ToLower();
                if (string.IsNullOrEmpty(key))
                {
                    continue;
                }
                table.FillContentsWithFilteredSheetData(contents, keyValue, i);
                
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
        
        DateTime? GetModifiedTimeFromEditor(string filePath, TS[] lists)
        {
#if UNITY_EDITOR
            if (string.IsNullOrEmpty(filePath))
            {
                return default;
            }
            var latestAsset = lists.OrderByDescending(asset => asset.ModifiedDate).FirstOrDefault();
            if (latestAsset != default
                && !string.IsNullOrEmpty(latestAsset.ModifiedDate))
            {
                return DateTime.Parse(latestAsset.ModifiedDate);
            }
#endif
            return default;
        }
        
        protected TS[] LoadEditorCuvModelLists(string buildPath)
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
        
        string ICuvDoc.GetCmsName() => "Excel";
        
        string ICuvDoc.GetDocUrl() => GetDocUrl();
        
        string GetDocUrl() => "https://github.com/IShix-g/CMSuniVortex/blob/main/docs/IntegrationWithExcel.md";
    }
}