
using UnityEngine;
using UnityEngine.UI;
using CMSuniVortex;

namespace Test.Excel
{
    public sealed class CuvModelTest : CuvModel<EModel, EModelExcelCuvReference>
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