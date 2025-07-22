
using System;
using UnityEngine;

namespace CMSuniVortex
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class CuvAllLanguageAttribute : PropertyAttribute
    {
        public bool IsDefaultApplicationLanguage { get; private set; }
        
        public CuvAllLanguageAttribute(bool isDefaultApplicationLanguage = false)
        {
            IsDefaultApplicationLanguage = isDefaultApplicationLanguage;
        }
    }
}