
using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using CMSuniVortex.Tasks;

#if UNITY_EDITOR
using System.Net.Http;
using UnityEditor;
#endif

namespace CMSuniVortex.Cockpit
{
    [CuvClient("Cockpit")]
    public abstract class CockpitCuvClient<T, TS> : CuvClient<T, TS>, ICuvDoc, ICuvUpdateChecker where T : CockpitModel where TS : CockpitCuvModelList<T>
    {
        [SerializeField, CuvOpenUrl] string _baseUrl;
        [SerializeField] string _apiKey;
        [SerializeField] string _modelName;
        
        public string BaseUrl
        {
            get => _baseUrl;
            set => _baseUrl = value;
        }
        public string ApiKey
        {
            get => _apiKey;
            set => _apiKey = value;
        }
        public string ModelName
        {
            get => _modelName;
            set => _modelName = value;
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
        
        protected abstract JsonConverter<T> CreateConverter();
        
        public override bool CanILoad()
        {
            if (string.IsNullOrEmpty(_baseUrl))
            {
                Debug.LogError("Please input the BaseUrl.");
                return false;
            }
            if (string.IsNullOrEmpty(_apiKey))
            {
                Debug.LogError("Please input the ApiKey.");
                return false;
            }
            if (string.IsNullOrEmpty(_modelName))
            {
                Debug.LogError("Please input the ModelName.");
                return false;
            }
            return true;
        }

        protected override IEnumerator LoadModels(int currentRound, string buildPath, SystemLanguage language, Action<T[], string> onSuccess = default, Action<string> onError = default)
        {
#if UNITY_EDITOR
            var url = ConvertToLoadAllItemsUrl(language);
            using var request = UnityWebRequest.Get(url);
            request.SetRequestHeader("Content-Type", "application/json; charset=UTF-8");
            request.SetRequestHeader("api-key", _apiKey);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var settings = new JsonSerializerSettings();
                settings.Converters.Add(CreateConverter());
                var models = JsonConvert.DeserializeObject<T[]>(request.downloadHandler.text, settings);
                foreach (var model in models)
                {
                    model.SetData(_baseUrl, buildPath);
                    if (model.ResourcesLoadCoroutines != default)
                    {
                        foreach (var enumerator in model.ResourcesLoadCoroutines)
                        {
                            yield return enumerator;
                        }
                    }
                }
                
                onSuccess?.Invoke(models, typeof(TS).Name + "_" + language);
            }
            else
            {
                onError?.Invoke(request.error);
                Debug.LogError("LoadModels error: " + request.error);
            }
#else
        return default;
#endif
        }
        
        string ConvertToLoadAllItemsUrl(SystemLanguage language)
            => Path.Combine(_baseUrl, "api/content/items/", $"{_modelName.Trim('/')}?locale={language}&sort=%7B_id%3A+1%7D");
        
        bool ICuvUpdateChecker.IsUpdateAvailable()
            => !string.IsNullOrEmpty(_baseUrl)
               && !string.IsNullOrEmpty(_apiKey)
               && !string.IsNullOrEmpty(_modelName);

        void ICuvUpdateChecker.CheckForUpdate(string buildPath, Action<bool, string> successAction, Action<string> failureAction)
        {
#if UNITY_EDITOR
            _updateTokenSource = new CancellationTokenSource();
            CheckForUpdate(buildPath)
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
                    },
                    cancellationToken: _updateTokenSource.Token);
#endif
        }

#if UNITY_EDITOR
        async Task<(bool HasUpdate, string Details)> CheckForUpdate(string buildPath)
        {
            var lists = GetLists(buildPath);
            if (lists == null || lists.Length == 0)
            {
                return (true, "Editor: Not Generated Yet");
            }

            var sb = new StringBuilder();
            var hasUpdate = false;

            foreach (var asset in lists)
            {
                var url = ConvertToLoadAllItemsUrl(asset.Language);
                using var request = UnityWebRequest.Get(url);
                request.SetRequestHeader("Content-Type", "application/json; charset=UTF-8");
                request.SetRequestHeader("api-key", _apiKey);
            
                await request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"LoadModelsInternal: {request.error}");
                    throw new HttpRequestException($"LoadModelsInternal: {request.error}");
                }

                var settings = new JsonSerializerSettings();
                settings.Converters.Add(CreateConverter());
                var results = JsonConvert.DeserializeObject<T[]>(request.downloadHandler.text, settings);
                
                sb.Append("- ");
                sb.Append(asset.Language);
                sb.Append(" -\n");
                
                foreach (var result in results)
                {
                    var modifiedDate = DateTime.Parse(result.ModifiedDate);
                    sb.Append("Key: ");
                    sb.Append(result.Key);
                    sb.Append(" | ");
                    sb.Append(" Server: ");
                    sb.Append(modifiedDate.ToString("MM/dd/yyyy HH:mm"));
                    sb.Append(" Editor: ");

                    var model = GetModel(asset, result.CockpitID);
                    if (model != default)
                    {
                        var modifiedDate2 = DateTime.Parse(model.ModifiedDate);
                        hasUpdate |= modifiedDate > modifiedDate2;
                        sb.Append(modifiedDate2.ToString("MM/dd/yyyy HH:mm"));
                    }
                    sb.Append("\n");
                }
            }

            Debug.Log(sb.ToString());

            return (hasUpdate, "Please check the Console for details.");
        }

        T GetModel(TS list, string targetCockpitID)
        {
            for (var i = 0; i < list.Length; i++)
            {
                var model = list.GetByIndex(i);
                if (model.CockpitID == targetCockpitID)
                {
                    return model;
                }
            }
            return default;
        }
#endif
        
        TS[] GetLists(string buildPath)
        {
#if UNITY_EDITOR
            if (Path.HasExtension(buildPath))
            {
                buildPath = Path.GetDirectoryName(buildPath);
            }
            return AssetDatabase.FindAssets("t:" + typeof(TS), new []{ buildPath })
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<TS>)
                .ToArray();
#else
            return default;
#endif
        }
        
        string ICuvDoc.GetCmsName() => "Cockpit";
        
        string ICuvDoc.GetDocUrl() => "https://github.com/IShix-g/CMSuniVortex/blob/main/docs/IntegrationWithCockpit.md";
    }
}