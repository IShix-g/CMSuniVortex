#if ENABLE_ADDRESSABLES
using CMSuniVortex.Addressable;

namespace CMSuniVortex.GoogleSheet
{
    public sealed class GoogleSheetCuvAddressableReference : CuvAddressableReference<GoogleSheetModel, GoogleSheetCuvModelList>
    {
        public string SheetName;
    }
}
#endif