
using CMSuniVortex;
using CMSuniVortex.Cockpit;
using Newtonsoft.Json;

namespace Tests
{
    // [CuvIgnore] // Enabling this attribute will exclude it from the Client drop-down.
    // [CuvDisplayName("YourCustomName")] // Enabling this attribute changes the name on the client drop-down.
    public sealed class CatDetailsCockpitCuvClient : CockpitCuvClient<CatDetails, CatDetailsCockpitCuvModelList>
    {
        protected override JsonConverter<CatDetails> CreateConverter()
            => new CuvModelConverter<CatDetails>();
    }
}