
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;

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
        
        public HashSet<ResourceLoadAction> ResourceLoadActions { get; private set; }
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

        public void AddAction(ResourceLoadAction action)
        {
            ResourceLoadActions ??= new HashSet<ResourceLoadAction>();
            ResourceLoadActions.Add(action);
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
            ResourceLoadActions = default;
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

        public bool ImageExists(string key)
        {
            var imagePath = GetImagePath(key);
            return !string.IsNullOrEmpty(imagePath);
        }
        
        public void LoadSprite(string key, Action<Sprite> successAction)
        {
#if UNITY_EDITOR
            var imagePath = GetImagePath(key);
            if (!string.IsNullOrEmpty(imagePath))
            {
                var task = new ResourceLoadAction(imagePath, path =>
                {
                    var asset = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                    successAction?.Invoke(asset);
                });
                AddAction(task);
            }
            else
            {
                successAction?.Invoke(default);
            }
#endif
        }

        public void LoadTexture(string key, Action<Texture2D> successAction)
        {
#if UNITY_EDITOR
            var imagePath = GetImagePath(key);
            if (!string.IsNullOrEmpty(imagePath))
            {
                var task = new ResourceLoadAction(imagePath, path =>
                {
                    var asset = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                    successAction?.Invoke(asset);
                });
                AddAction(task);
            }
            else
            {
                successAction?.Invoke(default);
            }
#endif
        }

#if ENABLE_ADDRESSABLES
        public void LoadSpriteReference(string key, Action<AssetReferenceSprite> completeAction)
        {
#if UNITY_EDITOR
            var imagePath = GetImagePath(key);
            if (!string.IsNullOrEmpty(imagePath))
            {
                AddressableActions ??= new HashSet<AddressableAction>();
                
                var task = new ResourceLoadAction(imagePath, path =>
                {
                    AddressableActions.Add(new AddressableAction(
                        AssetDatabase.AssetPathToGUID(path),
                        guid =>
                        {
                            completeAction?.Invoke(new AssetReferenceSprite(guid));
                        }));
                });
                AddAction(task);
            }
#endif
        }
        
        public void LoadTextureReference(string key, Action<AssetReferenceTexture2D> completeAction)
        {
#if UNITY_EDITOR
            var imagePath = GetImagePath(key);
            if (!string.IsNullOrEmpty(imagePath))
            {
                AddressableActions ??= new HashSet<AddressableAction>();
                
                var task = new ResourceLoadAction(imagePath, path =>
                {
                    AddressableActions.Add(new AddressableAction(
                        AssetDatabase.AssetPathToGUID(path),
                        guid =>
                        {
                            completeAction?.Invoke(new AssetReferenceTexture2D(guid));
                        }));
                });
                AddAction(task);
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
    }
}