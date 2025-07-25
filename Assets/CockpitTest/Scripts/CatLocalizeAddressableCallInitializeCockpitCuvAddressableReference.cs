
using CMSuniVortex.Cockpit;

namespace Tests
{
    public sealed class CatLocalizeAddressableCallInitializeCockpitCuvAddressableReference :
        CockpitCuvAddressableReference<CatLocalizeAddressableCallInitialize,
            CatLocalizeAddressableCallInitializeCockpitCuvModelList>
    {
        public override bool EnableAutoLocalization => false;
    }
}