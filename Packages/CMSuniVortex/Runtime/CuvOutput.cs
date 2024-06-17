
namespace CMSuniVortex
{
    /// <summary>
    /// Represents an output object used in the CMSuniVortex framework.
    /// </summary>
    public abstract class CuvOutput<TModel, TModelList, TReference> : ICuvOutput
        where TModel : ICuvModel
        where TModelList : CuvModelList<TModel>
        where TReference : CuvReference<TModel, TModelList>
    {
        public abstract void Select(string buildPath);
        public abstract void Deselect();
        public abstract void Generate(string buildPath, ICuvClient client, string[] listGuids);
        public abstract void Release();
    }
}