
using UnityEngine;
using UnityEngine.UI;

namespace Test.Cockpit
{
    [RequireComponent(typeof(Image))]
    public sealed class CuvLocalizedImageTest : CuvLocalizedTest
    {
        [SerializeField] Image _image;


        protected override void OnChangeLanguage(CatDetailsLocalize model)
        {
            _image.sprite = model.Image;
        }

        protected override void Reset()
        {
            base.Reset();
            _image = GetComponent<Image>();
        }
    }
}