
using UnityEngine;

namespace CMSuniVortex
{
    /// <summary>
    /// Represents a generic abstract class that manages a list of models.
    /// </summary>
    public interface ICuvModelList<T> where T : ICuvModel
    {
        SystemLanguage Language { get; }
        int Length { get; }
        T GetByIndex(int index);
        T GetByKey(string id);
        bool TryGetByKey(string id, out T model);
    }
}