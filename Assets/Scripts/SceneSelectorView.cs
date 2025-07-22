
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Test
{
    public sealed class SceneSelectorView : MonoBehaviour
    {
        [SerializeField] RectTransform _parent;
        [SerializeField] SceneSelectorViewButton _button;
        [SerializeField] SceneSelector _selector;

        bool _isFirst = true;
        
        void Start()
        {
            _button.gameObject.SetActive(false);
            foreach (var sceneName in _selector.ScenesNames)
            {
                Create(sceneName);
            }
        }
        
        SceneSelectorViewButton Create(string sceneName)
        {
            var go = _isFirst ? _button : Instantiate(_button, _parent);
            var isActive = SceneManager.GetActiveScene().name == sceneName;
            go.Initialize(sceneName, OnClicked, isActive);
            go.gameObject.SetActive(true);
            _isFirst = false;
            return go;
        }
        
        void OnClicked(SceneSelectorViewButton button)
        {
            _selector.LoadScene(button.SceneName);
        }
    }
}