#if ENABLE_ADDRESSABLES
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

namespace CMSuniVortex
{
    public interface ICuvAsyncReference<T, TS> : ICuvIdReference, ICuvKeyReference
        where T : ICuvModel 
        where TS : Object, ICuvModelList<T>
    {
        AssetReferenceT<TS> FindModelListById(string id);
    }
}
#endif