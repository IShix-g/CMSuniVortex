
using System.ComponentModel;
using CMSuniVortex;
using CMSuniVortex.Excel;

namespace Test
{
    // [CuvIgnore] // Enabling this attribute will exclude it from the Client drop-down.
    // [DisplayName("YourCustomName")] // Enabling this attribute changes the name on the client drop-down.
    public sealed class EModelExcelCuvClient : ExcelCuvClient<EModel, EModelExcelCuvModelList> {}
}