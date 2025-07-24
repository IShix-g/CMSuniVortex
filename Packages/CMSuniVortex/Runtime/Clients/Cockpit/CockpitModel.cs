
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if ENABLE_ADDRESSABLES
using CMSuniVortex.Addressable;
using UnityEngine.AddressableAssets;
#endif

namespace CMSuniVortex.Cockpit
{
    [Serializable]
    public abstract class CockpitModel : ICuvModel, IJsonDeserializerKeySettable, IJsonDeserializer
    #if ENABLE_ADDRESSABLES
    ,IAddressableModel
    #endif
    {
        public string Key;
        public string CockpitID;
        public string ModifiedDate;
        
        public string BaseUrl { get; private set; }
        public const string ApiEndPoint = "storage/uploads";
        public HashSet<IEnumerator> ResourcesLoadCoroutines { get; private set; }
#if ENABLE_ADDRESSABLES
        public HashSet<AddressableAction> AddressableActions { get; private set; }
#endif
        public string AssetSavePath { get; private set; }
        protected JObject JObject { get; private set; }

        protected abstract void OnDeserialize();

        public string GetKey() => Key;
        
        #region Editor
#if UNITY_EDITOR
        string _keyName;
        
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

        void IJsonDeserializerKeySettable.Set(string keyName) => _keyName = keyName;
        
        void IJsonDeserializer.Deserialize(JObject obj)
        {
            JObject = obj;
            Key = GetString(_keyName);
            CockpitID = GetString("_id");
            ModifiedDate = GetDateUtc("_modified");
            OnDeserialize();
        }

        void IDeserializationNotifier.OnDeserialized()
        {
            BaseUrl = default;
            ResourcesLoadCoroutines = default;
            AssetSavePath = default;
            JObject = default;
            
#if ENABLE_ADDRESSABLES
            AddressableActions = default;
#endif
        }
#else
    void IJsonDeserializerKeySettable.Set(string keyName){}
    void IJsonDeserializer.Deserialize(JObject obj){}
    void IDeserializationNotifier.OnDeserialized(){}
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
                imagePath = AppendImageExtension(imagePath, request);
                var texture = ((DownloadHandlerTexture) request.downloadHandler).texture;
                var imageBytes = default(byte[]);
                
                if (imagePath.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
                {
                    imageBytes = texture.EncodeToPNG();
                }
                else if (imagePath.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase)
                         || imagePath.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase))
                {
                    imageBytes = texture.EncodeToJPG();
                }
                else
                {
                    Debug.LogWarning("Image could not be saved. path: " + imagePath);
                    onSuccess?.Invoke(default);
                    yield break;
                }
                
                var fileName = Path.GetFileName(imagePath);
                var path = Path.Combine(AssetSavePath, fileName);
                File.WriteAllBytes(path, imageBytes);
                Object.DestroyImmediate(texture);
                AssetDatabase.ImportAsset(path);
                SetTextureTypeToSprite(path);
                onSuccess?.Invoke(path);
                
                var contentType = request.GetResponseHeader("Content-Type");
                Debug.Log("Image saved. [" + contentType + "] from URL: " + url + " to Path: " + path);
            }
            else
            {
                Debug.LogError("LoadSprite error imagePath: " + url + "  message: " + request.error);
            }
        }
        
        void SetTextureTypeToSprite(string assetPath)
        {
            var importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.SaveAndReimport();
            }
        }
#endif

#if ENABLE_ADDRESSABLES
        public void LoadSpriteReference(string key, Action<AssetReferenceSprite> completed)
        {
#if UNITY_EDITOR
            var imagePath = GetImagePath(key);
            if (!string.IsNullOrEmpty(imagePath))
            {
                AddCoroutine(LoadTextureCo(imagePath, path =>
                {
                    AddressableActions ??= new HashSet<AddressableAction>();
                    AddressableActions.Add(new AddressableAction(
                        AssetDatabase.AssetPathToGUID(path),
                        guid =>
                    {
                        completed?.Invoke(new AssetReferenceSprite(guid));
                    }));
                }));
            }
#endif
        }
        
        public void LoadTextureReference(string key, Action<AssetReferenceTexture2D> completed)
        {
#if UNITY_EDITOR
            var imagePath = GetImagePath(key);
            if (!string.IsNullOrEmpty(imagePath))
            {
                AddCoroutine(LoadTextureCo(imagePath, path =>
                {
                    AddressableActions ??= new HashSet<AddressableAction>();
                    AddressableActions.Add(new AddressableAction(
                        AssetDatabase.AssetPathToGUID(path),
                        guid =>
                        {
                            completed?.Invoke(new AssetReferenceTexture2D(guid));
                        }));
                }));
            }
#endif
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
        
        static string AppendImageExtension(string imagePath, UnityWebRequest request)
        {
            imagePath = imagePath.TrimEnd('/');
            var fileName = Path.GetFileName(imagePath);
            fileName = Regex.Replace(fileName, "[?<>:*|]", "");
            var directory = Path.GetDirectoryName(imagePath);
            imagePath = Path.Combine(directory, fileName);
            
            if (Path.HasExtension(imagePath)
                && !imagePath.EndsWith(".png", StringComparison.OrdinalIgnoreCase)
                && !imagePath.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase)
                && !imagePath.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase))
            {
                imagePath = imagePath.Replace(Path.GetExtension(imagePath), "");
            }

            if (Path.HasExtension(imagePath))
            {
                return imagePath;
            }
            
            var contentType = request.GetResponseHeader("Content-Type");
            if (contentType.Contains("image/png"))
            {
                imagePath += ".png";
            }
            else if (contentType.Contains("image/jpeg"))
            {
                imagePath += ".jpg";
            }
            else
            {
                Debug.LogWarning("Unknown image format from Content-Type header, encoding as PNG instead. path: " + imagePath);
                imagePath += ".png";
            }
            return imagePath;
        }
    }
}