#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEditor;
using CMSuniVortex.Addressable;

namespace Tests
{
    [CreateAssetMenu(fileName = "TestScriptableObject", menuName = "ScriptableObject/TestScriptableObject", order = 0)]
    public sealed class TestScriptableObject : ScriptableObject
    {
        [SerializeField] AssetReferenceT<CatDetailsCockpitCuvModelList> _assetRef;
        
        public void Reset()
        {
            var path = AssetDatabase.GUIDToAssetPath("c14ce121346a949128cb173ac749715f");
            var obj = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
            _assetRef.AttachAsset<CatDetailsCockpitCuvModelList>(obj);
        }
    }
}
#endif