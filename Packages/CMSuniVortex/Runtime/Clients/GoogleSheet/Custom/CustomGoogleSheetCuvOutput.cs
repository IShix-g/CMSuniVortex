
using System.IO;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CMSuniVortex.GoogleSheet
{
    public abstract class CustomGoogleSheetCuvOutput<TModel, TModelList, TReference> : CuvOutput<TModel, TModelList, TReference>
        where TModel : CustomGoogleSheetModel
        where TModelList : CustomGoogleSheetCuvModelList<TModel>
        where TReference : CustomGoogleSheetCuvReference<TModel, TModelList>
    {
        [SerializeField, CuvReadOnly] TReference _reference;
        
        public override TReference GetReference() => _reference;
        
        public override bool IsCompleted() => _reference != default;
        
        public override void Generate(string buildPath, ICuvClient client, string[] listGuids)
        {
#if UNITY_EDITOR
            var objs = new TModelList[listGuids.Length];
            for (var i = 0; i < listGuids.Length; i++)
            {
                var path = AssetDatabase.GUIDToAssetPath(listGuids[i]);
                objs[i] = AssetDatabase.LoadAssetAtPath<TModelList>(path);
            }
            
            var assetPath = GetReferencePath(buildPath);
            if (_reference == default)
            {
                _reference = ScriptableObject.CreateInstance<TReference>();
                _reference.hideFlags = HideFlags.NotEditable;
                AssetDatabase.CreateAsset(_reference, assetPath);
                Debug.Log("Generating Reference Assets. path:" + assetPath);
            }
            
            _reference.SetModelLists(objs);
            EditorUtility.SetDirty(_reference);
            AssetDatabase.SaveAssetIfDirty(_reference);
            Debug.Log("Loading Reference Assets. path:" + assetPath);
#endif
        }
        
        public override void Select(string buildPath)
        {
#if UNITY_EDITOR
            var buildFullPath =  GetReferencePath(buildPath);
            if (_reference == default)
            {
                _reference = AssetDatabase.LoadAssetAtPath<TReference>(buildFullPath);
            }
#endif
        }
        
        public override void Deselect() {}
        
        public override void Release()
        {
#if UNITY_EDITOR
            if (_reference == default)
            {
                return;
            }
            var assetPath = AssetDatabase.GetAssetPath(_reference);
            AssetDatabase.DeleteAsset(assetPath);
            _reference = default;
            Debug.Log("Release Completed. path:" + assetPath);
#endif
        }

        public override void ReloadReference(string buildPath)
        {
#if UNITY_EDITOR
            var buildFullPath =  GetReferencePath(buildPath);
            if (_reference == default
                && !File.Exists(buildFullPath))
            {
                _reference = AssetDatabase.LoadAssetAtPath<TReference>(buildFullPath);
            }
#endif
        }
        
        string GetReferencePath(string buildPath) => Path.Combine(buildPath, typeof(TReference).Name + ".asset");
    }
}