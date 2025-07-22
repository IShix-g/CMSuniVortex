
namespace CMSuniVortex
{
    /// <summary>
    /// Represents a generic abstract class that manages a list of models.
    /// </summary>
    public interface ICuvModelList<T> where T : ICuvModel
    {
        string CuvId { get; }
        int Length { get; }
        T GetFirst();
        T GetLast();
        T GetAt(int index);
        T GetByKey(string key);
        bool TryGetByKey(string key, out T model);
    }
}