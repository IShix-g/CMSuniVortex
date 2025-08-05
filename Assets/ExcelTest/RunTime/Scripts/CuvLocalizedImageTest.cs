
using UnityEngine;
using UnityEngine.UI;

namespace Test.Excel
{
    [RequireComponent(typeof(Image))]
    public sealed class CuvLocalizedImageTest : CuvLocalizedTest
    {
        [SerializeField] Image _image;


        protected override void OnChangeLanguage(EModelLocalized model)
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