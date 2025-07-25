
using System.ComponentModel;
using CMSuniVortex;
using CMSuniVortex.Cockpit;
using Newtonsoft.Json;

namespace Tests
{
    // [CuvIgnore] // Enabling this attribute will exclude it from the Client drop-down.
    // [DisplayName("YourCustomName")] // Enabling this attribute changes the name on the client drop-down.
    public sealed class CatDetailsCallInitializeCockpitCuvLocalizedClient : CockpitCuvLocalizedClient<CatDetailsCallInitialize, CatDetailsCallInitializeCockpitCuvModelList>
    {
        protected override JsonConverter<CatDetailsCallInitialize> CreateConverter()
            => new CuvModelConverter<CatDetailsCallInitialize>();
    }
}