
using UnityEngine;
using UnityEngine.UI;

namespace CMSuniVortex.Tests
{
    public sealed class CuvListTest : CuvList<TestCockpitModel, TestCockpitModelCockpitCuvReference>
    {
        [SerializeField] Text _text;
        
        void Start()
        {
            Debug.Log("CuvId: " + List.CuvId);
            _text.text = List.CuvId;
        }
    }
}