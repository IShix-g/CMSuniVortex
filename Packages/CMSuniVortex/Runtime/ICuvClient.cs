
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CMSuniVortex
{
    /// <summary>
    /// Represents a generic abstract client class that manages loading and serialization of model data.
    /// </summary>
    public interface ICuvClient
    {
        bool CanILoad();
        IEnumerator Load(string buildPath, IReadOnlyList<SystemLanguage> languages, Action<string[]> onLoaded);
        int GetRepeatCount();
        void Select(string assetPath);
        void Deselect();
    }
}