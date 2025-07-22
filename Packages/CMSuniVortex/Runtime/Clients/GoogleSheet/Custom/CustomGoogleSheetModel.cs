
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using Object = UnityEngine.Object;

#if ENABLE_ADDRESSABLES
using CMSuniVortex.Addressable;
using UnityEngine.AddressableAssets;
#endif

namespace CMSuniVortex.GoogleSheet
{
    [Serializable]
    public abstract class CustomGoogleSheetModel : GoogleSheetModelBase, IObjectDeserializer
#if ENABLE_ADDRESSABLES
        ,IAddressableModel
#endif
    {
        Dictionary<string, string> _contents;
        
        public HashSet<IEnumerator> ResourcesLoadCoroutines { get; private set; }
#if ENABLE_ADDRESSABLES
        public HashSet<AddressableAction> AddressableActions { get; private set; }
#endif
        public string AssetSavePath { get; private set; }
        
        protected abstract void OnDeserialize();
        
#if UNITY_EDITOR
        public void SetData(string assetSavePath) => AssetSavePath = assetSavePath;

        void IObjectDeserializer.Deserialize(Dictionary<string, string> contents)
        {
            _contents = contents;
            OnDeserialize();
        }

        void IDeserializationNotifier.OnDeserialized()
        {
            ResourcesLoadCoroutines = default;
            AssetSavePath = default;
            _contents = default;
        }
        
        public void AddCoroutine(IEnumerator enumerator)
        {
            ResourcesLoadCoroutines ??= new HashSet<IEnumerator>();
            ResourcesLoadCoroutines.Add(enumerator);
        }
#else
        void IObjectDeserializer.Deserialize(Dictionary<string, string> contents) {}
        void IDeserializationNotifier.OnDeserialized() {}
#endif
        
        public string GetString(string key)
            => _contents.TryGetValue(key, out var obj) ? obj : string.Empty;
        
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
        
        public void LoadSprite(string key, Action<Sprite> onSuccess = default)
        {
#if UNITY_EDITOR
            if (_contents.TryGetValue(key, out var obj)
                && !string.IsNullOrEmpty(obj))
            {
                AddCoroutine(LoadTextureCo(obj, path =>
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
            if (_contents.TryGetValue(key, out var obj)
                && !string.IsNullOrEmpty(obj))
            {
                AddCoroutine(LoadTextureCo(obj, path =>
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

#if ENABLE_ADDRESSABLES
        public void LoadSpriteReference(string key, Action<AssetReferenceSprite> completed)
        {
#if UNITY_EDITOR
            if (_contents.TryGetValue(key, out var obj)
                && !string.IsNullOrEmpty(obj))
            {
                AddCoroutine(LoadTextureCo(obj, path =>
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
            if (_contents.TryGetValue(key, out var obj)
                && !string.IsNullOrEmpty(obj))
            {
                AddCoroutine(LoadTextureCo(obj, path =>
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
        
#if UNITY_EDITOR
        public IEnumerator LoadTextureCo(string url, Action<string> onSuccess = default)
        {
            using var request = UnityWebRequestTexture.GetTexture(url);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                url = AppendImageExtension(url, request);
                var texture = ((DownloadHandlerTexture) request.downloadHandler).texture;
                var imageBytes = default(byte[]);
                
                if (url.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
                {
                    imageBytes = texture.EncodeToPNG();
                }
                else if (url.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase)
                         || url.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase))
                {
                    imageBytes = texture.EncodeToJPG();
                }
                else
                {
                    Debug.LogWarning("Image could not be saved. path: " + url);
                    onSuccess?.Invoke(default);
                    yield break;
                }

                var fileName = Path.GetFileName(url);
                var path = Path.Combine(AssetSavePath, fileName);
                File.WriteAllBytes(path, imageBytes);
                Object.DestroyImmediate(texture);
                AssetDatabase.ImportAsset(path);
                onSuccess?.Invoke(path);

                var contentType = request.GetResponseHeader("Content-Type");
                Debug.Log("Image saved. [" + contentType + "] from URL: " + url + " to Path: " + path);
            }
            else
            {
                Debug.LogError("LoadSprite error " + " / message: " + request.error);
            }
        }
#endif
        
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

        public void OnDeserialized()
        {
            throw new NotImplementedException();
        }
    }
}