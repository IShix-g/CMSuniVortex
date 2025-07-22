
using System.ComponentModel;
using CMSuniVortex;
using CMSuniVortex.Cockpit;
using Newtonsoft.Json;

namespace Test
{
    // [CuvIgnore] // Enabling this attribute will exclude it from the Client drop-down.
    // [DisplayName("YourCustomName")] // Enabling this attribute changes the name on the client drop-down.
    public sealed class CatDetailsLocalizeCockpitCuvClient : CockpitCuvLocalizedClient<CatDetailsLocalize, CatDetailsLocalizeCockpitCuvModelList>
    {
        protected override JsonConverter<CatDetailsLocalize> CreateConverter()
            => new CuvModelConverter<CatDetailsLocalize>();
    }
}