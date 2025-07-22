
using UnityEngine;
using UnityEngine.UI;

namespace CMSuniVortex.Tests
{
    [RequireComponent(typeof(Image))]
    public sealed class CuvLocalizedImageTest : CuvLocalizedTest
    {
        [SerializeField] Image _image;
        
        protected override void OnChangeLanguage(TestCockpitModel model)
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