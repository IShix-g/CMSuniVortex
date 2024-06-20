
namespace CMSuniVortex.GoogleSheet
{
    public class GoogleSheetCuvReference : CuvReference<GoogleSheetModel, GoogleSheetCuvModelList>
    {
        public string SheetName => GetList().SheetName;
    }
}