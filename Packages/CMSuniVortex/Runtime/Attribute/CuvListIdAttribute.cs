
using System;
using UnityEngine;

namespace CMSuniVortex
{
    /// <summary>
    /// Display a drop-down list of keys in CuvList.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class CuvListIdAttribute : PropertyAttribute
    {
        public readonly string ReferenceName;

        public CuvListIdAttribute(string referenceName)
        {
            ReferenceName = referenceName;
        }
    }
}