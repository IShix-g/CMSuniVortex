
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CMSuniVortex.Cockpit
{
    public abstract class CockpitCuvLocalizedClient<T, TS> 
        : CockpitCuvClientBase<T, TS>, ICuvLocalizedClient
        where T : CockpitModel 
        where TS : CockpitCuvModelList<T>
    {
        [SerializeField] SystemLanguage[] _languages;
        [SerializeField, Tooltip("Check this if the Locale set in the control panel corresponds to internationalization (i18n) codes rather than SystemLanguage.")] bool _useI18nCode;
        
        public override IReadOnlyList<string> GetCuvIds()
            => _languages.Select(language => language.ToString()).ToList();

        public SystemLanguage[] GetLanguages() => _languages;

        protected override string ConvertToLoadAllItemsUrl(string cuvId)
        {
            if (_useI18nCode
                && Enum.TryParse(cuvId, out SystemLanguage lang))
            {
                var langStg = ToI18nCode(lang);
                if (!string.IsNullOrEmpty(langStg))
                {
                    cuvId = langStg;
                }
            }
            return base.ConvertToLoadAllItemsUrl(cuvId);
        }
        
        public static string ToI18nCode(SystemLanguage language)
        {
            return language switch
            {
                SystemLanguage.Afrikaans => "af",
                SystemLanguage.Arabic => "ar",
                SystemLanguage.Basque => "eu",
                SystemLanguage.Belarusian => "be",
                SystemLanguage.Bulgarian => "bg",
                SystemLanguage.Catalan => "ca",
                SystemLanguage.Chinese => "zh",
                SystemLanguage.ChineseSimplified => "zh_Hant",
                SystemLanguage.ChineseTraditional => "zh_Hans",
                SystemLanguage.Czech => "cs",
                SystemLanguage.Danish => "da",
                SystemLanguage.Dutch => "nl",
                SystemLanguage.English => "en",
                SystemLanguage.Estonian => "et",
                SystemLanguage.Faroese => "fo",
                SystemLanguage.Finnish => "fi",
                SystemLanguage.French => "fr",
                SystemLanguage.German => "de",
                SystemLanguage.Greek => "el",
                SystemLanguage.Hebrew => "he",
                SystemLanguage.Hungarian => "hu",
                SystemLanguage.Icelandic => "is",
                SystemLanguage.Indonesian => "id",
                SystemLanguage.Italian => "it",
                SystemLanguage.Japanese => "ja",
                SystemLanguage.Korean => "ko",
                SystemLanguage.Latvian => "lv",
                SystemLanguage.Lithuanian => "lt",
                SystemLanguage.Norwegian => "no",
                SystemLanguage.Polish => "pl",
                SystemLanguage.Portuguese => "pt",
                SystemLanguage.Romanian => "ro",
                SystemLanguage.Russian => "ru",
                SystemLanguage.SerboCroatian => "sh",
                SystemLanguage.Slovak => "sk",
                SystemLanguage.Slovenian => "sl",
                SystemLanguage.Spanish => "es",
                SystemLanguage.Swedish => "sv",
                SystemLanguage.Thai => "th",
                SystemLanguage.Turkish => "tr",
                SystemLanguage.Ukrainian => "uk",
                SystemLanguage.Vietnamese => "vi",
                _ => string.Empty,
            };
        }
    }
}