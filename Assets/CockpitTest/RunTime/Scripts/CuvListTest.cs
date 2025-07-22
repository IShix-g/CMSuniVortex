
using UnityEngine;
using UnityEngine.UI;
using CMSuniVortex;
using Tests;

namespace Test.Cockpit
{
    public class CuvListTest : CuvList<CatDetails, CatDetailsCockpitCuvReference>
    {
        [SerializeField] Text _text;
        
        void Start()
        {
            Debug.Log("CuvId: " + List.CuvId);
            _text.text = List.CuvId;
        }
    }
}