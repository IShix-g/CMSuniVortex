
using UnityEngine;
using UnityEngine.UI;

namespace CMSuniVortex.Tests
{
    public sealed class CuvModelTest : CuvModel<TestCockpitModel, TestCockpitModelCockpitCuvReference>
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