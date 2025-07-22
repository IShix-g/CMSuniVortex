
using System;
using System.Collections;
using System.Collections.Generic;

namespace CMSuniVortex
{
    /// <summary>
    /// Represents a generic abstract client class that manages loading and serialization of model data.
    /// </summary>
    public interface ICuvClient
    {
        bool CanILoad();
        IEnumerator Load(string buildPath, Action<string[]> onLoaded);
        int GetRepeatCount();
        IReadOnlyList<string> GetCuvIds();
        void Select(string assetPath);
        void Deselect();
    }
}