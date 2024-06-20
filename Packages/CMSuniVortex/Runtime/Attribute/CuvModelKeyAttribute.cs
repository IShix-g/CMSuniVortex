
using System;
using UnityEngine;

namespace CMSuniVortex
{
    /// <summary>
    /// Display a drop-down list of keys in CuvModel.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class CuvModelKeyAttribute : PropertyAttribute
    {
        public readonly string ReferenceName;

        public CuvModelKeyAttribute(string referenceName)
        {
            ReferenceName = referenceName;
        }
    }
}