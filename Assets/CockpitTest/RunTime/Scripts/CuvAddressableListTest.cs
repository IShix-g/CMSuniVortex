
using UnityEngine;
using UnityEngine.UI;
using CMSuniVortex;
using Tests;

namespace Test.Cockpit
{
    public sealed class CuvAddressableListTest : CuvAddressableList<CatAddressableDetails, CatAddressableDetailsCockpitCuvModelList, CatAddressableDetailsCockpitCuvAddressableReference>
    {
        [SerializeField] Text _text;
        
        protected override void Awake()
        {
            base.Awake();
            Debug.Log("addressables - Initialize: " + IsLoaded);
        }

        void Start()
        {
            Debug.Log("addressables - IsLoading: " + IsLoading);
        }

        protected override void OnLoaded()
        {
            Debug.Log("addressables - CuvId: " + ActiveList.CuvId);
            Debug.Log("addressables - Initialize " + IsLoaded);
            Debug.Log("addressables - IsLoading: " + IsLoading);
            _text.text = ActiveList.CuvId;
        }
    }
}