
using System;
using UnityEngine;

namespace CMSuniVortex
{
    /// <summary>
    /// Display dialog to select file path
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class CuvFilePathAttribute : PropertyAttribute
    {
        public readonly string Extension;

        public CuvFilePathAttribute(string extension) => Extension = extension;
    }
}