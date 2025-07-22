
namespace CMSuniVortex
{
    /// <summary>
    /// Managing a reference to a list of models.
    /// </summary>
    public interface ICuvReference<T> : ICuvKeyReference, ICuvIdReference
        where T : ICuvModel
    {
        ICuvModelList<T> GetModelList(string id);
    }
}