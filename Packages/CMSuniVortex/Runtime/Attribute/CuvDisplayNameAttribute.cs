
using System;

namespace CMSuniVortex
{
    /// <summary>
    /// Change the name on the CuvImporter dropdown
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class CuvDisplayNameAttribute : Attribute
    {
        public string DisplayName { get; }

        public CuvDisplayNameAttribute(string displayName)
        {
            DisplayName = displayName;
        }
    }
}