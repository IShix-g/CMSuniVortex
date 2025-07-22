#if ENABLE_ADDRESSABLES
namespace CMSuniVortex.Tests
{
    public abstract class CuvAddressableLocalizedTest : CuvAddressableLocalized<TestCockpitModelCockpitCuvAddressableReference>
    {
        protected abstract void OnChangeLanguage(TestCockpitModel catDetails);
        
        protected override void OnChangeLanguage(TestCockpitModelCockpitCuvAddressableReference reference, string key)
        {
            if (reference.TryGetByKey(key, out var model))
            {
                OnChangeLanguage(model);
            }
        }
    }
}
#endif