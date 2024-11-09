
using System;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace CMSuniVortex.GoogleSheet
{
    public abstract class GoogleSheetCuvClientBase<T, TS> : CuvClient<T, TS>, ICuvDoc, ICuvUpdateChecker where T : GoogleSheetModelBase, new() where TS : GoogleSheetCuvModelListBase<T>
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
        CancellationTokenSource _updateTokenSource;
#endif

        protected override void OnDeselect()
        {
            base.OnDeselect();
#if UNITY_EDITOR
            _updateTokenSource?.SafeCancelAndDispose();
#endif
        }
        
        public override bool CanILoad()
        {
#if UNITY_EDITOR
            if (!base.CanILoad())
            {
                return false;
            }
            if (string.IsNullOrEmpty(_sheetUrl))
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
            if (!GoogleSheetService.IsSheetUrlValid(_sheetUrl))
            {
                Debug.LogError("Could not convert Sheet Url to Sheet Id, please check if the URL is correct.");
                return false;
            }
            return true;
#else
            return false;
#endif
        }
        
        public virtual string GetCmsName() => "Google Sheet";

        public virtual string GetDocUrl() => "https://github.com/IShix-g/CMSuniVortex/blob/main/docs/IntegrationWithGoogleSheet.md";
        
        protected abstract TS[] LoadEditorCuvModelLists(string buildPath);
        
        public virtual bool IsUpdateAvailable()
            => !string.IsNullOrEmpty(_sheetUrl)
               && !string.IsNullOrEmpty(_jsonKeyPath);

        public virtual void CheckForUpdate(string buildPath, Action<bool, string> successAction, Action<string> failureAction)
        {
#if UNITY_EDITOR
            _updateTokenSource = new CancellationTokenSource();
            var lists = LoadEditorCuvModelLists(buildPath);
            var modifiedTime = GetModifiedTimeFromEditor(_sheetUrl, buildPath, lists);
            GoogleSheetService.CheckForUpdate(_sheetUrl, _jsonKeyPath, modifiedTime, _updateTokenSource.Token)
                .ContinueOnMainThread(
                    onSuccess: task =>
                    {
                        var result = task.Result;
                        successAction?.Invoke(result.HasUpdate, result.Details);
                    },
                    onError: error =>
                    {
                        if (error != default)
                        {
                            failureAction?.Invoke("See console for error details.");
                            throw error;
                        }
                        
                        failureAction?.Invoke("Unknown error. Please try again later.");
                    },
                    onCompleted: () =>
                    {
                        _updateTokenSource?.Dispose();
                        _updateTokenSource = default;
                    });
#endif
        }

        DateTime? GetModifiedTimeFromEditor(string sheetUrl, string buildPath, TS[] lists)
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
            var latestAsset = lists.OrderByDescending(asset => asset.ModifiedDate).FirstOrDefault();
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