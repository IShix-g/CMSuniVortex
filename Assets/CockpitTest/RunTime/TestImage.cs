
using UnityEngine;
using UnityEngine.UI;

namespace Tests
{
    [RequireComponent(typeof(Image))]
    public sealed class TestImage : TestCatDetailsCockpitCuvComponent
    {
        [SerializeField] Image _image;


        protected override void OnChangeLanguage(CatDetails model)
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