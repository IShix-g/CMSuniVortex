
using CMSuniVortex;

namespace Test.Excel
{
    public abstract class CuvAddressableLocalizedTest : CuvAddressableLocalized<EModelLocalizedAddressableExcelCuvAddressableReference>
    {
        protected abstract void OnChangeLanguage(EModelLocalizedAddressable catDetails);
        
        protected override void OnChangeLanguage(EModelLocalizedAddressableExcelCuvAddressableReference reference, string key)
        {
            if (reference.TryGetByKey(key, out var model))
            {
                OnChangeLanguage(model);
            }
        }
    }
}