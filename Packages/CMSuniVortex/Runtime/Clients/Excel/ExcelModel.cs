
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

#if ENABLE_ADDRESSABLES
using CMSuniVortex.Addressable;
using UnityEngine.AddressableAssets;
#endif

namespace CMSuniVortex.Excel
{
    [Serializable]
    public abstract class ExcelModel : ICuvModel, IObjectDeserializer
#if ENABLE_ADDRESSABLES
        ,IAddressableModel
#endif
    {
        public string Key;
        
        public string GetKey() => Key;
        
        Dictionary<string, string> _contents;
        
        public HashSet<ResourceLoadAction> ResourceLoadActions { get; private set; }
#if ENABLE_ADDRESSABLES
        public HashSet<AddressableAction> AddressableActions { get; private set; }
#endif
        
        protected abstract void OnDeserialize();
        
#if UNITY_EDITOR

        void IObjectDeserializer.Deserialize(Dictionary<string, string> contents)
        {
            _contents = contents;
            OnDeserialize();
        }

        void IDeserializationNotifier.OnDeserialized()
        {
            _contents = default;
        }
        
        public void AddAction(ResourceLoadAction action)
        {
            ResourceLoadActions ??= new HashSet<ResourceLoadAction>();
            ResourceLoadActions.Add(action);
        }
#else
        void IObjectDeserializer.Deserialize(Dictionary<string, string> contents) {}
        void IDeserializationNotifier.OnDeserialized() {}
#endif
        
        public string GetString(string key)
            => _contents.TryGetValue(key, out var obj) ? obj : string.Empty;
        
        public T GetEnum<T>(string key, IReadOnlyDictionary<string, T> maps) where T : struct, Enum
        {
            var result = GetEnumOrNull<T>(key, maps);
            return result ?? default;
        }
        
        public bool TryGetEnum<T>(string key, IReadOnlyDictionary<string, T> maps, out T enumValue) where T : struct, Enum
        {
            var result = GetEnumOrNull<T>(key, maps);
            enumValue = result ?? default;
            return result.HasValue;
        }
        
        public T? GetEnumOrNull<T>(string key, IReadOnlyDictionary<string, T> maps) where T : struct, Enum
            => maps.TryGetValue(GetString(key), out var enumValue)
                ? (T?)enumValue
                : null;
        
        public T GetEnum<T>(string key) where T : struct, Enum
            => TryGetValue(key, out var value)
                ? Enum.TryParse<T>(value, out var val) ? val : default
                : default;
        
        public bool GetBool(string key)
            => TryGetValue(key, out var value)
               && string.Equals(value, "TRUE", StringComparison.OrdinalIgnoreCase);

        public int GetInt(string key)
            => TryGetValue(key, out var value)
                ? int.Parse(value)
                : default;

        public long GetLong(string key)
            => TryGetValue(key, out var value)
                ? long.Parse(value)
                : default;

        public float GetFloat(string key)
            => TryGetValue(key, out var value)
                ? float.Parse(value)
                : default;

        public double GetDouble(string key)
            => TryGetValue(key, out var value)
                ? double.Parse(value)
                : default;

        public string GetDate(string key)
            => TryGetValue(key, out var value)
               && DateTimeOffset.TryParse(value, out var date)
                ? date.ToString("o")
                : default;

        public bool TryGetValue(string key, out string value)
        {
            if (_contents.TryGetValue(key, out var obj)
                && !string.IsNullOrEmpty(obj))
            {
                value = obj;
                return true;
            }
            value = string.Empty;
            return false;
        }

        public string GetImagePath(string key)
        {
            if (_contents.TryGetValue(key, out var imagePath)
                && !string.IsNullOrEmpty(imagePath))
            {
                return imagePath;
            }
            return string.Empty;
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
                LoadImage<Sprite>(imagePath, successAction);
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
                LoadImage<Texture2D>(imagePath, successAction);
            }
            else
            {
                successAction?.Invoke(default);
            }
#endif
        }

        public void LoadImage<T>(string imagePath, Action<T> successAction) where T : Object
        {
#if UNITY_EDITOR
            var task = new ResourceLoadAction(imagePath, path =>
            {
                var asset = AssetDatabase.LoadAssetAtPath<T>(path);
                successAction?.Invoke(asset);
            });
            AddAction(task);
#endif
        }
        
#if ENABLE_ADDRESSABLES
        public void LoadSpriteReference(string key, Action<AssetReferenceSprite> completeAction)
        {
#if UNITY_EDITOR
            var imagePath = GetImagePath(key);
            if (!string.IsNullOrEmpty(imagePath))
            {
                LoadImageGuid(imagePath, guid => completeAction?.Invoke(new AssetReferenceSprite(guid)));
            }
            else
            {
                completeAction?.Invoke(default);
            }
#endif
        }
        
        public void LoadTextureReference(string key, Action<AssetReferenceTexture2D> completeAction)
        {
#if UNITY_EDITOR
            var imagePath = GetImagePath(key);
            if (!string.IsNullOrEmpty(imagePath))
            {
                LoadImageGuid(imagePath, guid => completeAction?.Invoke(new AssetReferenceTexture2D(guid)));
            }
            else
            {
                completeAction?.Invoke(default);
            }
#endif
        }
        
        public void LoadImageGuid(string imagePath, Action<string> completeAction)
        {
#if UNITY_EDITOR
            AddressableActions ??= new HashSet<AddressableAction>();

            var task = new ResourceLoadAction(imagePath, path =>
            {
                AddressableActions.Add(new AddressableAction(
                    AssetDatabase.AssetPathToGUID(path),
                    guid => completeAction?.Invoke(guid)));
            });
            AddAction(task);
#endif
        }
#endif
    }
}