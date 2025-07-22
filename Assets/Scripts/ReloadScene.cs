
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Test
{
    [RequireComponent(typeof(Button))]
    public sealed class ReloadScene : MonoBehaviour
    {
        [SerializeField] Button _button;

        void Start()
        {
            _button.onClick.AddListener(ClickButton);
        }

        void ClickButton()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        void Reset()
        {
            _button = GetComponent<Button>();
        }
    }
}