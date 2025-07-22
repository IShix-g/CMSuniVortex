
using UnityEngine;
using UnityEngine.UI;
using CMSuniVortex;

namespace Test.GoogleSheet
{
    public sealed class CuvModelTest : CuvModel<Meta, MetaCustomGoogleSheetCuvReference>
    {
        [SerializeField] Text _text;
        [SerializeField] Image _image;

        void Start()
        {
            Debug.Log("ModelId: " + Model.Key);
            _text.text = Model.Text;
            _image.sprite = Model.Image;
        }
    }
}