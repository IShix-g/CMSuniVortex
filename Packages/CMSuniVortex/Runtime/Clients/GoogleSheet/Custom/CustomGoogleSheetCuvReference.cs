
namespace CMSuniVortex.GoogleSheet
{
    public abstract class CustomGoogleSheetCuvReference<T, TS> : CuvReference<T, TS> where T : CustomGoogleSheetModel where TS : CustomGoogleSheetCuvModelList<T> {}
}