
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using Object = UnityEngine.Object;

namespace CMSuniVortex.GoogleSheet
{
    [Serializable]
    public abstract class CustomGoogleSheetModel : ICuvModel
    {
        public string Key;
        
        public HashSet<IEnumerator> ResourcesLoadCoroutines { get; private set; }
        public string AssetSavePath { get; private set; }
        
        public abstract void Deserialize(Dictionary<string, string> models);
        
        public string GetID() => Key;

        #region Editor
#if UNITY_EDITOR
        public void SetData(string assetSavePath)
        {
            AssetSavePath = assetSavePath;
        }

        public void AddCoroutine(IEnumerator enumerator)
        {
            ResourcesLoadCoroutines ??= new HashSet<IEnumerator>();
            ResourcesLoadCoroutines.Add(enumerator);
        }
#endif
        #endregion
        
        public void LoadSprite(Dictionary<string, string> models, string key, Action<Sprite> onSuccess = default)
        {
#if UNITY_EDITOR
            if (models.TryGetValue(key, out var obj)
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

        public void LoadTexture(Dictionary<string, string> models, string key, Action<Texture2D> onSuccess = default)
        {
#if UNITY_EDITOR
            if (models.TryGetValue(key, out var obj)
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

#if UNITY_EDITOR
        public IEnumerator LoadTextureCo(string url, Action<string> onSuccess = default)
        {
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
                Debug.LogError("LoadSprite error " + " / message: " + request.error);
            }
        }
#endif
    }
}