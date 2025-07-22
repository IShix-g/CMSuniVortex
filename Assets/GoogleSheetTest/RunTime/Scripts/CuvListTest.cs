
using UnityEngine;
using UnityEngine.UI;
using CMSuniVortex;

namespace Test.GoogleSheet
{
    public class CuvListTest : CuvList<Meta, MetaCustomGoogleSheetCuvReference>
    {
        [SerializeField] Text _text;
        
        void Start()
        {
            Debug.Log("CuvId: " + List.CuvId);
            _text.text = List.CuvId;
        }
    }
}