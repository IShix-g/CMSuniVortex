
using CMSuniVortex;

namespace Test.Cockpit
{
    public abstract class CuvLocalizedTest : CuvLocalized<CatDetailsLocalizeCockpitCuvReference>
    {
        protected abstract void OnChangeLanguage(CatDetailsLocalize catDetails);
        
        protected override void OnChangeLanguage(CatDetailsLocalizeCockpitCuvReference reference, string key)
        {
            if (reference.TryGetByKey(key, out var model))
            {
                OnChangeLanguage(model);
            }
        }
    }
}