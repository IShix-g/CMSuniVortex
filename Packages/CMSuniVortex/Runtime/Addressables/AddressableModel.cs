#if ENABLE_ADDRESSABLES
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

namespace CMSuniVortex.Addressable
{
    [Serializable]
    public sealed class AddressableModel<T, TS> where T : ICuvModel where TS : Object, ICuvModelList<T>
    {
        public SystemLanguage Language;
        public AssetReferenceT<TS> List;
    }
}
#endif