
using UnityEngine;
using UnityEngine.UI;

namespace Test.Cockpit
{
    [RequireComponent(typeof(Text))]
    public sealed class CuvAddressableLocalizedTextTest : CuvAddressableLocalizedTest
    {
        [SerializeField] Text _text;

        protected override void OnChangeLanguage(CatAddressableDetailsLocalize model)
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