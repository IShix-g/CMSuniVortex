
using System.ComponentModel;
using CMSuniVortex;
using CMSuniVortex.Cockpit;
using Newtonsoft.Json;

namespace CMSuniVortex.Tests
{
    // [CuvIgnore] // Enabling this attribute will exclude it from the Client drop-down.
    // [DisplayName("YourCustomName")] // Enabling this attribute changes the name on the client drop-down.
    public sealed class TestCockpitModelCockpitCuvAddressableLocalizedClient : CockpitCuvAddressableLocalizedClient<TestCockpitModel, TestCockpitModelCockpitCuvModelList>
    {
        protected override JsonConverter<TestCockpitModel> CreateConverter()
            => new CuvModelConverter<TestCockpitModel>();
    }
}