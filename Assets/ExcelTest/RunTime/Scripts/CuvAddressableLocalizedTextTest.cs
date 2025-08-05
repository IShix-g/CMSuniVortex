
using UnityEngine;
using UnityEngine.UI;

namespace Test.Excel
{
    [RequireComponent(typeof(Text))]
    public sealed class CuvAddressableLocalizedTextTest : CuvAddressableLocalizedTest
    {
        [SerializeField] Text _text;

        protected override void OnChangeLanguage(EModelLocalizedAddressable model)
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