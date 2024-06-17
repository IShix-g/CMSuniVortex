
using System;

namespace CMSuniVortex
{
    /// <summary>
    /// Attribute to indicate that the class should not be included in the automatic import process.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class CuvIgnoreAttribute : Attribute {}
}