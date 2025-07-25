
using System.ComponentModel;
using CMSuniVortex;
using CMSuniVortex.Cockpit;
using Newtonsoft.Json;

namespace Tests
{
    // [CuvIgnore] // Enabling this attribute will exclude it from the Client drop-down.
    // [DisplayName("YourCustomName")] // Enabling this attribute changes the name on the client drop-down.
    public sealed class CatLocalizeAddressableCallInitializeCockpitCuvAddressableLocalizedClient : CockpitCuvAddressableLocalizedClient<CatLocalizeAddressableCallInitialize, CatLocalizeAddressableCallInitializeCockpitCuvModelList>
    {
        protected override JsonConverter<CatLocalizeAddressableCallInitialize> CreateConverter()
            => new CuvModelConverter<CatLocalizeAddressableCallInitialize>();
    }
}