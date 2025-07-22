
using CMSuniVortex;
using CMSuniVortex.Cockpit;
using Newtonsoft.Json;

namespace Tests
{
    // [CuvIgnore] // Enabling this attribute will exclude it from the Client drop-down.
    // [DisplayName("YourCustomName")] // Enabling this attribute changes the name on the client drop-down.
    public sealed class CatAddressableDetailsCockpitCuvAddressableClient : CockpitCuvAddressableClient<CatAddressableDetails, CatAddressableDetailsCockpitCuvModelList>
    {
        protected override JsonConverter<CatAddressableDetails> CreateConverter()
            => new CuvModelConverter<CatAddressableDetails>();
    }
}