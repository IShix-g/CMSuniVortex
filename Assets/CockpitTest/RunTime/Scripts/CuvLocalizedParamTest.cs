
using UnityEngine;
using UnityEngine.UI;
using CMSuniVortex;

namespace Test.Cockpit
{
    [RequireComponent(typeof(Text))]
    public sealed class CuvLocalizedParamTest : CuvLocalizedTest
    {
        [SerializeField] Text _text;

        protected override void OnChangeLanguage(CatDetailsLocalize model)
        {
            _text.text = model.Param.SetParam("number", 5);
        }
        
        protected override void Reset()
        {
            base.Reset();
            _text = GetComponent<Text>();
        }
    }
}