
namespace CMSuniVortex.Tests
{
    public abstract class CuvLocalizedTest : CuvLocalized<TestCockpitModelCockpitCuvReference>
    {
        protected abstract void OnChangeLanguage(TestCockpitModel catDetails);
        
        protected override void OnChangeLanguage(TestCockpitModelCockpitCuvReference reference, string key)
        {
            if (reference.TryGetByKey(key, out var model))
            {
                OnChangeLanguage(model);
            }
        }
    }
}