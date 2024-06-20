
using CMSuniVortex.Compornents;

namespace Tests
{
    public abstract class TestCatDetailsCockpitCuvAddressableComponent : CuvAsyncComponent<CatAddressableDetailsCockpitCuvAddressableReference>
    {
        protected abstract void OnChangeLanguage(CatAddressableDetails catDetails);
        
        protected override void OnChangeLanguage(CatAddressableDetailsCockpitCuvAddressableReference reference, string key)
        {
            if (reference.GetList().TryGetByKey(key, out var model))
            {
                OnChangeLanguage(model);
            }
        }
    }
}