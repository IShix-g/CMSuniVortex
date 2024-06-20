
using UnityEngine;
using UnityEngine.UI;

namespace Tests
{
    [RequireComponent(typeof(Text))]
    public sealed class TestAddressableText : TestCatDetailsCockpitCuvAddressableComponent
    {
        [SerializeField] Text _text;

        protected override void OnChangeLanguage(CatAddressableDetails model)
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