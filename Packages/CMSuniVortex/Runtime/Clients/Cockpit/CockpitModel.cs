
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using Object = UnityEngine.Object;

namespace CMSuniVortex.Cockpit
{
    [Serializable]
    public abstract class CockpitModel : ICuvModel, IJsonDeserializer
    {
        public string ID;
        public string ModifiedDate;
        
        public string BaseUrl { get; private set; }
        public const string ApiEndPoint = "storage/uploads";
        public HashSet<IEnumerator> ResourcesLoadCoroutines { get; private set; }
        public string AssetSavePath { get; private set; }
        protected JObject JObject { get; private set; }

        protected abstract void OnDeserialize();

        public string GetID() => ID;
        
        #region Editor
#if UNITY_EDITOR
        public void SetData(string basePath, string assetSavePath)
        {
            BaseUrl = basePath;
            AssetSavePath = assetSavePath;
        }

        public void AddCoroutine(IEnumerator enumerator)
        {
            ResourcesLoadCoroutines ??= new HashSet<IEnumerator>();
            ResourcesLoadCoroutines.Add(enumerator);
        }

        void IJsonDeserializer.Deserialize(JObject obj)
        {
            JObject = obj;
            ID = GetString("_id");
            ModifiedDate = GetDateUtc("_modified");
            OnDeserialize();
        }

        void IJsonDeserializer.Deserialized()
        {
            BaseUrl = default;
            ResourcesLoadCoroutines = default;
            AssetSavePath = default;
            JObject = default;
        }
#else
    void IJsonDeserializer.Deserialize(JObject obj){}

    void IJsonDeserializer.Deserialized(){}
#endif
        #endregion

        public string GetString(string key) => Get<string>(key);

        public string[] GetStrings(string key) => Gets<string>(key);

        public long GetLong(string key) => Get<long>(key);

        public long[] GetLongs<T>(string key) => Gets<long>(key);

        public int GetInt(string key) => Get<int>(key);

        public int[] GetInts<T>(string key) => Gets<int>(key);

        public float GetFloat(string key) => Get<float>(key);

        public float[] GetFloats(string key) => Gets<float>(key);

        public double GetDouble(string key) => Get<double>(key);

        public double[] GetDoubles(string key) => Gets<double>(key);

        public bool GetBool(string key) => Get<bool>(key);

        public bool[] GetBools(string key) => Gets<bool>(key);

        public string GetSelect(string key) => GetString(key);

        public T GetSelect<T>(string key) where T : struct, Enum
        {
            if (JObject.TryGetValue(key, out var value)
                && value.Type != JTokenType.Null
                && Enum.TryParse(value.Value<string>(), out T e))
            {
                return e;
            }

            return default;
        }

        public string[] GetTag(string key) => GetStrings(key);

        public T[] GetTag<T>(string key) where T : struct, Enum
        {
            var list = GetStrings(key);
            if (list.Length == 0)
            {
                return Array.Empty<T>();
            }

            var result = new List<T>();
            foreach (var item in list)
            {
                if (Enum.TryParse<T>(item, out var enumValue))
                {
                    result.Add(enumValue);
                }
            }

            return result.ToArray();
        }

        public Color GetColor(string key)
            => JObject[key] != default
               && JObject[key].Type != JTokenType.Null
               && ColorUtility.TryParseHtmlString(JObject[key].Value<string>(), out var color)
                ? color
                : default;

        public Color[] GetColors(string key)
            => GetStrings(key)
                .Select(item => ColorUtility.TryParseHtmlString(item, out var color) ? color : default)
                .Where(color => color != default)
                .ToArray();

        public string GetDate(string key)
            => JObject[key] != default
               && JObject[key].Type != JTokenType.Null
               && DateTimeOffset.TryParse(JObject[key].Value<string>(), out var date)
                ? date.ToString("o")
                : default;

        public string[] GetDates(string key)
        {
            var tokens = JObject.SelectToken(key);
            return tokens is JArray array
                ? array.Where(item => item.Type != JTokenType.Null)
                    .Select(item => DateTimeOffset.TryParse(item.Value<string>(), out var date)
                        ? date.ToString("o")
                        : null)
                    .Where(item => !string.IsNullOrEmpty(item))
                    .ToArray()
                : Array.Empty<string>();
        }

        public string GetImagePath(string key)
        {
            if (JObject.TryGetValue(key, out var valueToken)
                && valueToken.Type != JTokenType.Null
                && valueToken["path"] != default
                && valueToken["path"].Type != JTokenType.Null)
            {
                return valueToken["path"].Value<string>();
            }
            return string.Empty;
        }

        public string[] GetImagePaths(string key)
        {
            var tokens = JObject.SelectToken(key);
            return tokens is JArray array
                ? array.Where(item => item.Type != JTokenType.Null)
                    .Where(item => item["path"] != default)
                    .Select(item => item["path"].Value<string>())
                    .Where(path => !string.IsNullOrEmpty(path))
                    .ToArray()
                : Array.Empty<string>();
        }

        public void LoadSprite(string key, Action<Sprite> onSuccess = default)
        {
#if UNITY_EDITOR
            var imagePath = GetImagePath(key);
            if (!string.IsNullOrEmpty(imagePath))
            {
                AddCoroutine(LoadTextureCo(imagePath, path =>
                {
                    var asset = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                    onSuccess?.Invoke(asset);
                }));
            }
            else
            {
                onSuccess?.Invoke(default);
            }
#endif
        }

        public void LoadTexture(string key, Action<Texture2D> onSuccess = default)
        {
#if UNITY_EDITOR
            var imagePath = GetImagePath(key);
            if (!string.IsNullOrEmpty(imagePath))
            {
                AddCoroutine(LoadTextureCo(imagePath, path =>
                {
                    var asset = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                    onSuccess?.Invoke(asset);
                }));
            }
            else
            {
                onSuccess?.Invoke(default);
            }
#endif
        }

#if UNITY_EDITOR
        public IEnumerator LoadTextureCo(string imagePath, Action<string> onSuccess = default)
        {
            var url = Path.Combine(BaseUrl, ApiEndPoint, imagePath.TrimStart('/'));
            using var request = UnityWebRequestTexture.GetTexture(url);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var texture = ((DownloadHandlerTexture) request.downloadHandler).texture;
                var imageBytes = texture.EncodeToPNG();
                var fileName = Path.GetFileName(url);
                var path = Path.Combine(AssetSavePath, fileName);
                File.WriteAllBytes(path, imageBytes);
                Object.DestroyImmediate(texture);
                AssetDatabase.ImportAsset(path);
                onSuccess?.Invoke(path);
            }
            else
            {
                Debug.LogError("LoadSprite error imagePath: " + url + "  message: " + request.error);
            }
        }
#endif

        T Get<T>(string key)
            => JObject.TryGetValue(key, out var value)
               && value.Type != JTokenType.Null
                ? value.Value<T>()
                : default;

        T[] Gets<T>(string key)
        {
            var tokens = JObject.SelectToken(key);
            if (tokens is JArray array)
            {
                return array.Where(x => x.Type != JTokenType.Null)
                    .Select(x => x.Value<T>())
                    .ToArray();
            }

            return Array.Empty<T>();
        }

        public string GetDateUtc(string key)
        {
            if (JObject[key] != default
                && JObject[key].Type != JTokenType.Null
                && long.TryParse(JObject[key].Value<string>(), out var created))
            {
                var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                return epoch.AddSeconds(created).ToLocalTime().ToString("o");
            }

            return default;
        }
    }
}