
using CMSuniVortex;

namespace Test.GoogleSheet
{
    public abstract class CuvAddressableLocalizedTest : CuvAddressableLocalized<MetaLocalizeAddressableCustomGoogleSheetCuvAddressableReference>
    {
        protected abstract void OnChangeLanguage(MetaLocalizeAddressable catDetails);
        
        protected override void OnChangeLanguage(MetaLocalizeAddressableCustomGoogleSheetCuvAddressableReference reference, string key)
        {
            if (reference.TryGetByKey(key, out var model))
            {
                OnChangeLanguage(model);
            }
        }
    }
}