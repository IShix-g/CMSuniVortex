
using CMSuniVortex;
using CMSuniVortex.Cockpit;
using Newtonsoft.Json;

namespace Test
{
    public sealed class CatDetailsCockpitCuvClient : CockpitCuvClient<CatDetails, CatDetailsCockpitCuvModelList>
    {
        protected override JsonConverter<CatDetails> CreateConverter()
            => new CuvModelConverter<CatDetails>();
    }
}