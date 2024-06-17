
using System;
using UnityEngine;

namespace CMSuniVortex
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class CuvFilePathAttribute : PropertyAttribute
    {
        public readonly string Extension;

        public CuvFilePathAttribute(string extension) => Extension = extension;
    }
}