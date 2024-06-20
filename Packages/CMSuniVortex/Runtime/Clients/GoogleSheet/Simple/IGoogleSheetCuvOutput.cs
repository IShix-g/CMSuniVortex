
namespace CMSuniVortex.GoogleSheet
{
    // This is because this interface needs to be a generic representation of which models and which listings to use.
    public interface IGoogleSheetCuvOutput<TModel, TModelList, TReference> : ICuvOutput
        where TModel : GoogleSheetModel
        where TModelList : GoogleSheetCuvModelList
        where TReference : GoogleSheetCuvReference {}
}