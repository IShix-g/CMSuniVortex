
using CMSuniVortex;

namespace Test.Excel
{
    public abstract class CuvLocalizedTest : CuvLocalized<EModelLocalizedExcelCuvReference>
    {
        protected abstract void OnChangeLanguage(EModelLocalized catDetails);
        
        protected override void OnChangeLanguage(EModelLocalizedExcelCuvReference reference, string key)
        {
            if (reference.TryGetByKey(key, out var model))
            {
                OnChangeLanguage(model);
            }
        }
    }
}