
using System.ComponentModel;
using CMSuniVortex.Cockpit;

namespace Tests
{
    // [CuvIgnore] // Enabling this attribute will exclude it from the Client drop-down.
    // [DisplayName("YourCustomName")] // Enabling this attribute changes the name on the client drop-down.
    public sealed class CatDetailsCockpitCuvOutput : CockpitCuvOutput<CatDetails, CatDetailsCockpitCuvModelList, CatDetailsCockpitCuvReference> {}
}