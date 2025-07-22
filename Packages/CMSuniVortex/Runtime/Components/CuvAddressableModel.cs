#if ENABLE_ADDRESSABLES
using UnityEngine;
using UnityEngine.Assertions;
using Object = UnityEngine.Object;

namespace CMSuniVortex
{
    public abstract class CuvAddressableModel<T, TS, TR> : CuvAddressableList<T, TS, TR>
        where T : ICuvModel
        where TS : Object, ICuvModelList<T>
        where TR : ScriptableObject, ICuvAsyncReference<T, TS>
    {
        [SerializeField, CuvModelKey("_reference")] string _key;
        
        public string Key => _key;
        
        public T ActiveModel
        {
            get
            {
                Assert.IsFalse(string.IsNullOrEmpty(_key), "Please set the Key from Inspector.");
                return ActiveList.GetByKey(_key);
            }
        }
    }
}
#endif