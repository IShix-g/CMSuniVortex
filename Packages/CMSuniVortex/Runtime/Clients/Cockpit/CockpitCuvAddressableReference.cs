#if ENABLE_ADDRESSABLES
using CMSuniVortex.Addressable;

namespace CMSuniVortex.Cockpit
{
    public abstract class CockpitCuvAddressableReference<T, TS> 
        : CuvAddressableReference<T, TS> 
        where T : CockpitModel 
        where TS : CockpitCuvModelList<T> {}
}
#endif