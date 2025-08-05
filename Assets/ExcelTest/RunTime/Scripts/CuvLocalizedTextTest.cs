
using UnityEngine;
using UnityEngine.UI;

namespace Test.Excel
{
    [RequireComponent(typeof(Text))]
    public sealed class CuvLocalizedTextTest : CuvLocalizedTest
    {
        [SerializeField] Text _text;

        protected override void OnChangeLanguage(EModelLocalized model)
        {
            _text.text = model.Text;
        }
        
        protected override void Reset()
        {
            base.Reset();
            _text = GetComponent<Text>();
        }
    }
}