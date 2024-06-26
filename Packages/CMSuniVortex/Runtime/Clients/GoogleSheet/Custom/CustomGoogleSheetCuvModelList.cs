
namespace CMSuniVortex.GoogleSheet
{
    public abstract class CustomGoogleSheetCuvModelList<T> : CuvModelList<T> where T : CustomGoogleSheetModel
    {
        public string SheetID;
        public string ModifiedDate;
    }
}