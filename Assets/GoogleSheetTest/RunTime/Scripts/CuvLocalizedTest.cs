
using CMSuniVortex;

namespace Test.GoogleSheet
{
    public abstract class CuvLocalizedTest : CuvLocalized<MetaLocalizeCustomGoogleSheetCuvReference>
    {
        protected abstract void OnChangeLanguage(MetaLocalize catDetails);
        
        protected override void OnChangeLanguage(MetaLocalizeCustomGoogleSheetCuvReference reference, string key)
        {
            if (reference.TryGetByKey(key, out var model))
            {
                OnChangeLanguage(model);
            }
        }
    }
}