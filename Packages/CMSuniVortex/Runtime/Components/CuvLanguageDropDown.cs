
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CMSuniVortex
{
    public sealed class CuvLanguageDropDown : CuvLanguages
    {
        [SerializeField] Dropdown _dropdown;

        protected override void OnInitialized()
        {
            var options = new List<Dropdown.OptionData>();
            foreach (var language in Languages)
            {
                var languageString = language.ToString();
                var data = new Dropdown.OptionData(languageString);
                options.Add(data);
            }
            _dropdown.options = options;
            _dropdown.value = GetLanguageIndex(ActiveLanguage);
            _dropdown.onValueChanged.AddListener(OnValueChanged);
        }

        void OnValueChanged(int index)
        {
            var language = GetLanguageAt(index);
            ChangeLanguage(language);
        }

        void Reset() => _dropdown = GetComponent<Dropdown>();
    }
}