
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CMSuniVortex
{
    public abstract class CuvList<T, TS> : MonoBehaviour
        where T : ICuvModel
        where TS : ScriptableObject, ICuvReference<T>
    {
        [SerializeField] TS _reference;
        [SerializeField, CuvListId("_reference")] string _id;
        
        public TS Reference => _reference;
        
        public string Id => _id;

        public ICuvModelList<T> List
        {
            get
            {
                Assert.IsFalse(string.IsNullOrEmpty(_id), "Please set the Id from Inspector.");
                return _reference.GetModelList(_id);
            }
        }

        protected virtual void Reset()
        {
#if UNITY_EDITOR
            _reference = AssetDatabase.FindAssets("t:" + typeof(ScriptableObject))
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<ScriptableObject>)
                .OfType<TS>()
                .FirstOrDefault();
            
            if (_reference != default)
            {
                var ids = _reference.GetIds();
                _id = ids.Length > 0 ? ids[0] : string.Empty;
            }
#endif
        }
    }
}