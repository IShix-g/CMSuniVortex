
using UnityEngine;
using UnityEngine.UI;

namespace CMSuniVortex.Tests
{
    [RequireComponent(typeof(Text))]

    public sealed class CuvLocalizedParamTest : CuvLocalizedTest
    {
        [SerializeField] Text _text;

        protected override void OnChangeLanguage(TestCockpitModel model)
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
