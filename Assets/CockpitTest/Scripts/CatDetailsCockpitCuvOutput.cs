
using CMSuniVortex.Cockpit;

namespace Test
{
    // [CuvIgnore] // Enabling this attribute will exclude it from the Client drop-down.
    // [CuvDisplayName("YourCustomName")] // Enabling this attribute changes the name on the client drop-down.
    public sealed class CatDetailsCockpitCuvOutput : CockpitCuvOutput<CatDetails, CatDetailsCockpitCuvModelList, CatDetailsCockpitCuvReference> {}
}