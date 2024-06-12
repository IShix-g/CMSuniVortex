
using System;
using System.Collections;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace CMSuniVortex.Cockpit
{
    public abstract class CockpitCuvClient<T, TS> : CuvClient<T, TS>, ICuvDoc where T : CockpitModel where TS : CockpitCuvModelList<T>
    {
        [SerializeField] string _baseUrl;
        [SerializeField] string _apiKey;
        [SerializeField] string _modelName;
        
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
            var url = Path.Combine(_baseUrl, "api/content/items/", _modelName.Trim('/') + "?locale=" + language);
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
                    if (model.ResourcesLoadCoroutines.Count <= 0)
                    {
                        continue;
                    }

                    model.SetData(_baseUrl, buildPath);
                    if (model.ResourcesLoadCoroutines != default)
                    {
                        foreach (var enumerator in model.ResourcesLoadCoroutines)
                        {
                            yield return enumerator;
                        }
                    }
                    
                    ((IJsonDeserializer)model).Deserialized();
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
        
        string ICuvDoc.GetCmsName() => "Cockpit";
        
        string ICuvDoc.GetDocUrl() => "https://github.com/IShix-g/CMSuniVortex/blob/main/docs/IntegrationWithCockpit.md";
    }
}