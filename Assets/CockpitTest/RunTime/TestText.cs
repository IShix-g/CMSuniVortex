
using UnityEngine;
using UnityEngine.UI;

namespace Tests
{
    [RequireComponent(typeof(Text))]
    public sealed class TestText : TestCatDetailsCockpitCuvComponent
    {
        [SerializeField] Text _text;

        protected override void OnChangeLanguage(CatDetails model)
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