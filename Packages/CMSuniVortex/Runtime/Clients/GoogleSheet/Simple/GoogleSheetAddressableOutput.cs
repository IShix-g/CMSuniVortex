#if ENABLE_ADDRESSABLES
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using CMSuniVortex.Addressable;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CMSuniVortex.GoogleSheet
{
    public sealed class GoogleSheetAddressableOutput : CuvAddressableOutput<GoogleSheetModel, GoogleSheetCuvModelList, GoogleSheetCuvAddressableReference>
    {
        [SerializeField, CuvReadOnly] GoogleSheetCuvAddressableReference[] _references;
        
        public override GoogleSheetCuvAddressableReference GetReference() => _references[0];
        
        public override bool IsCompleted() => _references is {Length: > 0};

        public override void Generate(string buildPath, ICuvClient client, string[] listGuids)
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
            
            foreach (var obj in objs)
            {
                AddressableHelper.CreateGroupIfNotExists(GetGroupName(obj.Language), AddressableSettings, true);
            }
            
            var current = AssetDatabase.FindAssets("t:" + typeof(GoogleSheetCuvAddressableReference), new[] {buildPath})
                .Select(AssetDatabase.GUIDToAssetPath)
                .ToArray();
            var references = new List<GoogleSheetCuvAddressableReference>();
            foreach (var entry in sheetNameToObjects)
            {
                var sheetName = entry.Key;
                var sheetObjects = entry.Value.ToArray();
                
                var reference = default(GoogleSheetCuvAddressableReference);
                var assetPath = GetReferencePath(buildPath, sheetName);
                if (current is {Length: > 0}
                    && current.Any(x => x == assetPath))
                {
                    reference = AssetDatabase.LoadAssetAtPath<GoogleSheetCuvAddressableReference>(assetPath);
                }
                if (reference == default)
                {
                    reference = ScriptableObject.CreateInstance<GoogleSheetCuvAddressableReference>();
                    reference.hideFlags = HideFlags.NotEditable;
                    AssetDatabase.CreateAsset(reference, assetPath);
                    Debug.Log($"Generated Reference Asset for sheet {sheetName}. path: {assetPath}");
                }
                
                var models = new AddressableModel<GoogleSheetModel, GoogleSheetCuvModelList>[sheetObjects.Length];

                for (var i = 0; i < sheetObjects.Length; i++)
                {
                    var obj = sheetObjects[i];
                    var groupName = GetGroupName(obj.Language);
                    AddressableHelper.AddTo(groupName, obj, default, AddressableSettings.Labels);
                    var model = new AddressableModel<GoogleSheetModel, GoogleSheetCuvModelList>
                    {
                        Language = obj.Language,
                        List = new AssetReferenceT<GoogleSheetCuvModelList>(listGuids[i])
                    };
                    models[i] = model;
                }
                
                reference.SetModelLists(models);
                
                EditorUtility.SetDirty(reference);
                AssetDatabase.SaveAssetIfDirty(reference);
                references.Add(reference);
                Debug.Log($"Loaded Reference Asset for sheet {sheetName}. path: {assetPath}");
            }
            _references = references.ToArray();
#endif
        }

        public override void Select(string buildPath)
        {
#if UNITY_EDITOR
            if (_references == default
                || _references.Length == 0)
            {
                _references = AssetDatabase.FindAssets("t:" + typeof(GoogleSheetCuvAddressableReference), new[] { buildPath })
                    .Select(AssetDatabase.GUIDToAssetPath)
                    .Select(AssetDatabase.LoadAssetAtPath<GoogleSheetCuvAddressableReference>)
                    .Where(x => x != default)
                    .ToArray();
            }
#endif
        }
        public override void Deselect() {}
        
        public override void Release()
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

        public override void ReloadReference(string buildPath)
        {
#if UNITY_EDITOR
            if (_references == default
                || _references.Length == 0)
            {
                _references = AssetDatabase.FindAssets("t:" + typeof(GoogleSheetCuvAddressableReference), new[] { buildPath })
                    .Select(AssetDatabase.GUIDToAssetPath)
                    .Select(AssetDatabase.LoadAssetAtPath<GoogleSheetCuvAddressableReference>)
                    .Where(x => x != default)
                    .ToArray();
            }
#endif
        }

        public static string GetReferencePath(string buildPath, string sheetName) => Path.Combine(buildPath, nameof(GoogleSheetCuvAddressableReference) + "_" + sheetName + ".asset");
    }
}
#endif