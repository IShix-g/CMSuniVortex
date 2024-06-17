
using System;
using UnityEngine;

namespace CMSuniVortex
{
    /// <summary>
    /// Disable field editing
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class CuvReadOnlyAttribute : PropertyAttribute {}
}