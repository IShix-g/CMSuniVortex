
using System;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine.Networking;

namespace CMSuniVortex.Editor
{
    sealed class CheckVersion
    {
        
        internal static IEnumerator GetVersionOnServer(string gitUrl, string packageName, string defaultBranch, Action<string> onSuccess, Action onFailed = default)
        {
            var branchName = string.Empty;
            yield return GetVersionOrBranchFromPackageID(packageName, branch => branchName = branch);
            if (!string.IsNullOrEmpty(branchName))
            {
                gitUrl = gitUrl.Replace(defaultBranch, branchName);
            }
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
        
        static IEnumerator GetVersionOrBranchFromPackageID(string packageId, Action<string> onSuccess)
        {
            var request = Client.List();

            while (!request.IsCompleted)
            {
                yield return null;
            }
            
            foreach (var result in request.Result)
            {
                if (!result.packageId.Contains(packageId))
                {
                    continue;
                }
                var index = result.packageId.IndexOf('#');
                if (index != -1)
                {
                    var version = result.packageId.Substring(index + 1);
                    onSuccess.Invoke(version);
                }
                break;
            }
            onSuccess.Invoke(string.Empty);
        }
    }
}