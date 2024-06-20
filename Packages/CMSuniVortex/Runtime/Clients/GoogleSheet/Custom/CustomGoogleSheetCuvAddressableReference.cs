#if ENABLE_ADDRESSABLES
using CMSuniVortex.Addressable;

namespace CMSuniVortex.GoogleSheet
{
    public abstract class CustomGoogleSheetCuvAddressableReference<T, TS> : CuvAddressableReference<T, TS> where T : CustomGoogleSheetModel where TS : CustomGoogleSheetCuvModelList<T> {}
}
#endif