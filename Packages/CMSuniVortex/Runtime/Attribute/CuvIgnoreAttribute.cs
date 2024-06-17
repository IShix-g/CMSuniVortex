
using System;

namespace CMSuniVortex
{
    /// <summary>
    /// Specify if you do not want to be included in the CuvImporter drop-down
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class CuvIgnoreAttribute : Attribute {}
}