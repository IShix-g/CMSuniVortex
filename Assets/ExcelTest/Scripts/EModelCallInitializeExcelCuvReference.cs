
using CMSuniVortex.Excel;

namespace Test
{
    public sealed class EModelCallInitializeExcelCuvReference
        : ExcelCuvReference<EModelCallInitialize, EModelCallInitializeExcelCuvModelList>
    {
        public override bool EnableAutoLocalization => false;
    }
}