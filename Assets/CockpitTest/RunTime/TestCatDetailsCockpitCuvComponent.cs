
using CMSuniVortex.Compornents;

namespace Tests
{
    public abstract class TestCatDetailsCockpitCuvComponent : CuvComponent<CatDetailsCockpitCuvReference>
    {
        protected abstract void OnChangeLanguage(CatDetails catDetails);
        
        protected override void OnChangeLanguage(CatDetailsCockpitCuvReference reference, string key)
        {
            if (reference.TryGetByKey(key, out var model))
            {
                OnChangeLanguage(model);
            }
        }
    }
}