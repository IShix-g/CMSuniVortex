
using CMSuniVortex;
using CMSuniVortex.Cockpit;
using Newtonsoft.Json;

namespace Tests
{
    // [CuvIgnore] // Enabling this attribute will exclude it from the Client drop-down.
    // [CuvDisplayName("YourCustomName")] // Enabling this attribute changes the name on the client drop-down.
    public sealed class CatAddressableDetailsCockpitCuvClient : CockpitCuvClient<CatAddressableDetails, CatAddressableDetailsCockpitCuvModelList>
    {
        protected override JsonConverter<CatAddressableDetails> CreateConverter()
            => new CuvModelConverter<CatAddressableDetails>();
    }
}