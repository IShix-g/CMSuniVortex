
using CMSuniVortex;
using UnityEngine;
using UnityEngine.UI;

namespace Tests
{
    [RequireComponent(typeof(Text))]
    public sealed class TestParam : TestCatDetailsCockpitCuvComponent
    {
        [SerializeField] Text _text;

        protected override void OnChangeLanguage(CatDetails model)
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