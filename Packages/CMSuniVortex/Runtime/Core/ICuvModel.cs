
using System.Collections;
using System.Collections.Generic;

namespace CMSuniVortex
{
    public interface ICuvModel
    {
        string GetID();
        HashSet<IEnumerator> ResourcesLoadCoroutines { get; }
    }
}