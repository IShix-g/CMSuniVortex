
using System;

namespace CMSuniVortex
{
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