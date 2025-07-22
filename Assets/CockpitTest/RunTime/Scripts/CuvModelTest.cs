
using UnityEngine;
using UnityEngine.UI;
using CMSuniVortex;
using Tests;

namespace Test.Cockpit
{
    public sealed class CuvModelTest : CuvModel<CatDetails, CatDetailsCockpitCuvReference>
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