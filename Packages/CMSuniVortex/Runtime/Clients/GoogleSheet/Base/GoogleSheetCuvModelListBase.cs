
namespace CMSuniVortex.GoogleSheet
{
    public abstract class GoogleSheetCuvModelListBase<T> : CuvModelList<T> where T : GoogleSheetModelBase
    {
        public string ModifiedDate;
    }
}