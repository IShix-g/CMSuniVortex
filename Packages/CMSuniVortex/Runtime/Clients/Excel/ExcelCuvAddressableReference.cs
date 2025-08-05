#if ENABLE_ADDRESSABLES
using CMSuniVortex.Addressable;

namespace CMSuniVortex.Excel
{
    public abstract class ExcelCuvAddressableReference<T, TS>
        : CuvAddressableReference<T, TS> 
        where T : ExcelModel 
        where TS : ExcelCuvModelList<T> {}
}
#endif