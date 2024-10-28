
using System;
using System.Collections;
using System.IO;
using UnityEngine;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine.Networking;

namespace CMSuniVortex.Editor
{
    sealed class CheckVersion
    {
        internal static IEnumerator GetVersionOnServer(string gitUrl, Action<string> onSuccess, Action onFailed = default)
        {
            using var request = UnityWebRequest.Get(gitUrl);
            yield return request.SendWebRequest();
            
            if (request.result == UnityWebRequest.Result.Success)
            {
                var version = GetVersionByJson(request.downloadHandler.text);
                onSuccess?.Invoke(version);
            }
            else
            {
                Debug.LogError("GetVersion error: " + request.error);
                onFailed?.Invoke();
            }
        }
        
        internal static string GetCurrent(string packagePath)
        {
            var path = Path.Combine(packagePath, "package.json");
            var json = File.ReadAllText(path);
            return GetVersionByJson(json);
        }

        internal static void OpenPackageManager()
        {
            EditorApplication.ExecuteMenuItem("Window/Package Manager");
        }
        
        static string GetVersionByJson(string json)
        {
            var obj = JObject.Parse(json);
            return obj.TryGetValue("version", out var value) && value.Type != JTokenType.Null ? value.Value<string>() : "--";
        }
    }
}