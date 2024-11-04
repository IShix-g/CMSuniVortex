
namespace CMSuniVortex.GoogleSheet
{
    public abstract class CustomGoogleSheetCuvModelList<T> : GoogleSheetCuvModelListBase<T> where T : CustomGoogleSheetModel
    {
        public string SheetID;
    }
}