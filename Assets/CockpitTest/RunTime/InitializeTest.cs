
using UnityEngine;

namespace Tests
{
    public sealed class InitializeTest : MonoBehaviour
    {
        [SerializeField] CatAddressableDetailsCockpitCuvAddressableReference _reference;

        async void Start()
        {
            Debug.Log("Start Initialize flag: " + _reference.IsInitialized);
            if (!_reference.IsInitialized)
            {
                await _reference.InitializeAsync();
            }
            Debug.Log("IsLoaded : " + (_reference.GetList().GetByIndex(0).Text));
        }
    }
}