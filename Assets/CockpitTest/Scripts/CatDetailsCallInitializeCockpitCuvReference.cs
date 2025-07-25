
using CMSuniVortex.Cockpit;

namespace Tests
{
    public sealed class CatDetailsCallInitializeCockpitCuvReference : CockpitCuvReference<CatDetailsCallInitialize,
        CatDetailsCallInitializeCockpitCuvModelList>
    {
        public override bool EnableAutoLocalization => false;
    }
}