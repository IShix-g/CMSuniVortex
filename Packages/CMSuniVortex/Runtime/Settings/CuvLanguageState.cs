
using System;
using UnityEngine;

namespace CMSuniVortex
{
    [Serializable]
    public sealed class CuvLanguageState
    {
        [SerializeField] bool _isActive;
        [SerializeField] SystemLanguage _language;
        
        public bool IsActive => _isActive;
        public SystemLanguage Language => _language;
        
        public CuvLanguageState(bool isActive, SystemLanguage language)
        {
            _isActive = isActive;
            _language = language;
        }
    }
}