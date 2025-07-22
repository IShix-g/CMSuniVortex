
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Test
{
    public sealed class SceneSelectorViewButton : MonoBehaviour
    {
        [SerializeField] Text _text;
        [SerializeField] Button _button;
        [SerializeField] Outline _outline;
        
        public string SceneName { get; private set; }

        Action<SceneSelectorViewButton> _clickedAction;
        
        void Start() => _button.onClick.AddListener(ClickButton);

        public void Initialize(string sceneName, Action<SceneSelectorViewButton> clickedAction, bool isActive)
        {
            SceneName = sceneName;
            _clickedAction = clickedAction;
            _text.text = sceneName;
            _outline.enabled = isActive;
        }

        void ClickButton() => _clickedAction?.Invoke(this);
    }
}