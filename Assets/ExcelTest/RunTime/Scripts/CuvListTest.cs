
using UnityEngine;
using UnityEngine.UI;
using CMSuniVortex;

namespace Test.Excel
{
    public class CuvListTest : CuvList<EModel, EModelExcelCuvReference>
    {
        [SerializeField] Text _text;
        
        void Start()
        {
            Debug.Log("CuvId: " + List.CuvId);
            _text.text = List.CuvId;
        }
    }
}