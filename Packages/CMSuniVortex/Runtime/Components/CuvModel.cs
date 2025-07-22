
using UnityEngine;
using UnityEngine.Assertions;

namespace CMSuniVortex
{
    public abstract class CuvModel<T, TS> : CuvList<T, TS>
        where T : ICuvModel
        where TS : ScriptableObject, ICuvReference<T>
    {
        [SerializeField, CuvModelKey("_reference")] string _key;
        
        public string Key => _key;
        
        public T Model
        {
            get
            {
                Assert.IsFalse(string.IsNullOrEmpty(_key), "Please set the Key from Inspector.");
                return List.GetByKey(_key);
            }
        }
    }
}