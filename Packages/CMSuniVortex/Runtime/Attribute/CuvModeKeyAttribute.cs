
using System;
using UnityEngine;

namespace CMSuniVortex
{
    /// <summary>
    /// Display a drop-down list of keys in CuvModel.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class CuvModeKeyAttribute : PropertyAttribute
    {
        public readonly string ReferenceName;

        public CuvModeKeyAttribute(string referenceName)
        {
            ReferenceName = referenceName;
        }
    }
}