
using UnityEngine;

namespace CMSuniVortex
{
    /// <summary>
    /// Represents a generic abstract class that manages a list of models.
    /// </summary>
    public interface ICuvModelList
    {
        public SystemLanguage Language { get; }
        public int Length { get; }
    }
}