#if ENABLE_ADDRESSABLES
using System.IO;
using UnityEngine;
using UnityEngine.AddressableAssets;
using CMSuniVortex.Addressable;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CMSuniVortex.GoogleSheet
{
    public abstract class CustomGoogleSheetCuvAddressableOutput<TModel, TModelList, TReference> : CuvAddressableOutput<TModel, TModelList, TReference>
        where TModel : CustomGoogleSheetModel
        where TModelList : CustomGoogleSheetCuvModelList<TModel>
        where TReference : CustomGoogleSheetCuvAddressableReference<TModel, TModelList>
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
            
            foreach (var obj in objs)
            {
                AddressableHelper.CreateGroupIfNotExists(GetGroupName(obj.Language), AddressableSettings, true);
            }
            
            var assetPath = GetReferencePath(buildPath);
            if (_reference == default)
            {
                _reference = ScriptableObject.CreateInstance<TReference>();
                _reference.hideFlags = HideFlags.NotEditable;
                AssetDatabase.CreateAsset(_reference, assetPath);
                Debug.Log("Generating Reference Assets. path:" + assetPath);
            }

            var models = new AddressableModel<TModel, TModelList>[objs.Length];

            for (var i = 0; i < objs.Length; i++)
            {
                var obj = objs[i];
                AddressableHelper.AddTo(GetGroupName(obj.Language), obj);
                var model = new AddressableModel<TModel, TModelList>
                {
                    Language = obj.Language,
                    List = new AssetReferenceT<TModelList>(listGuids[i])
                };
                models[i] = model;
            }
            
            _reference.SetModelLists(models);
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
        
        public static string GetReferencePath(string buildPath) => Path.Combine(buildPath, typeof(TReference).Name + ".asset");
    }
}
#endif