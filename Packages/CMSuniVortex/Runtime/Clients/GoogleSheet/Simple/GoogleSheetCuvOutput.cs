
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CMSuniVortex.GoogleSheet
{
    public class GoogleSheetCuvOutput : IGoogleSheetCuvOutput<GoogleSheetModel, GoogleSheetCuvModelList, GoogleSheetCuvReference>
    {
        [SerializeField, CuvReadOnly] GoogleSheetCuvReference[] _references;
        
        public void Generate(string buildPath, ICuvClient client, string[] listGuids)
        {
#if UNITY_EDITOR
            var objs = new GoogleSheetCuvModelList[listGuids.Length];
            var sheetNameToObjects = new Dictionary<string, List<GoogleSheetCuvModelList>>();
            
            for (var i = 0; i < listGuids.Length; i++)
            {
                var path = AssetDatabase.GUIDToAssetPath(listGuids[i]);
                var obj = AssetDatabase.LoadAssetAtPath<GoogleSheetCuvModelList>(path);
                objs[i] = obj;
                
                if (!sheetNameToObjects.ContainsKey(obj.SheetName))
                {
                    sheetNameToObjects[obj.SheetName] = new List<GoogleSheetCuvModelList>();
                }
                sheetNameToObjects[obj.SheetName].Add(obj);
            }

            var current = AssetDatabase.FindAssets("t:" + typeof(GoogleSheetCuvReference), new[] {buildPath})
                .Select(AssetDatabase.GUIDToAssetPath)
                .ToArray();
            var references = new List<GoogleSheetCuvReference>();
            foreach (var entry in sheetNameToObjects)
            {
                var sheetName = entry.Key;
                var sheetObjects = entry.Value.ToArray();

                var reference = default(GoogleSheetCuvReference);
                var assetPath = GetReferencePath(buildPath, sheetName);
                if (current is {Length: > 0}
                    && current.Any(x => x == assetPath))
                {
                    reference = AssetDatabase.LoadAssetAtPath<GoogleSheetCuvReference>(assetPath);
                }
                if (reference == default)
                {
                    reference = ScriptableObject.CreateInstance<GoogleSheetCuvReference>();
                    reference.hideFlags = HideFlags.NotEditable;
                    AssetDatabase.CreateAsset(reference, assetPath);
                    Debug.Log($"Generated Reference Asset for sheet {sheetName}. path: {assetPath}");
                }
                
                reference.SetModelLists(sheetObjects);
                
                EditorUtility.SetDirty(reference);
                AssetDatabase.SaveAssetIfDirty(reference);
                references.Add(reference);
                Debug.Log($"Loaded Reference Asset for sheet {sheetName}. path: {assetPath}");
            }
            _references = references.ToArray();
#endif
        }
        
        public void Select(string buildPath)
        {
#if UNITY_EDITOR
            if (_references == default
                || _references.Length == 0)
            {
                _references = AssetDatabase.FindAssets("t:" + typeof(GoogleSheetCuvReference), new[] { buildPath })
                    .Select(AssetDatabase.GUIDToAssetPath)
                    .Select(AssetDatabase.LoadAssetAtPath<GoogleSheetCuvReference>)
                    .Where(x => x != default)
                    .ToArray();
            }
#endif
        }
        
        public void Deselect() {}
        
        public void Release()
        {
#if UNITY_EDITOR
            if (_references == default
                || _references.Length == 0)
            {
                return;
            }

            foreach (var reference in _references)
            {
                var assetPath = AssetDatabase.GetAssetPath(reference);
                AssetDatabase.DeleteAsset(assetPath);
                _references = default;
                Debug.Log("Release Completed. path:" + assetPath);
            }
#endif
        }
        
        string GetReferencePath(string buildPath, string sheetName) => Path.Combine(buildPath, nameof(GoogleSheetCuvReference) + "_" + sheetName + ".asset");
    }
}