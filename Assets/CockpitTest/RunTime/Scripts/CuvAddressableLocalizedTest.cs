
using CMSuniVortex;

namespace Test.Cockpit
{
    public abstract class CuvAddressableLocalizedTest : CuvAddressableLocalized<CatAddressableDetailsLocalizeCockpitCuvAddressableReference>
    {
        protected abstract void OnChangeLanguage(CatAddressableDetailsLocalize catDetails);
        
        protected override void OnChangeLanguage(CatAddressableDetailsLocalizeCockpitCuvAddressableReference reference, string key)
        {
            if (reference.TryGetByKey(key, out var model))
            {
                OnChangeLanguage(model);
            }
        }
    }
}