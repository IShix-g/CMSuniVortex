
using CMSuniVortex.Cockpit;
using Newtonsoft.Json;

namespace CMSuniVortex.Tests
{
    // Enabling this attribute will exclude it from the Client drop-down.
    // [IgnoreImporter]
    public sealed class TestCockpitCuvClient : CockpitCuvClient<TestCockpitModel, TestCockpitCuvModelList>
    {
        protected override JsonConverter<TestCockpitModel> CreateConverter()
            => new CuvModelConverter<TestCockpitModel>();
    }
}