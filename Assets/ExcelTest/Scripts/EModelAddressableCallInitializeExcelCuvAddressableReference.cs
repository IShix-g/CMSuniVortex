
using CMSuniVortex.Excel;

namespace Test
{
    public sealed class EModelAddressableCallInitializeExcelCuvAddressableReference
        : ExcelCuvAddressableReference<EModelAddressableCallInitialize,
            EModelAddressableCallInitializeExcelCuvModelList>
    {
        public override bool EnableAutoLocalization => false;
    }
}