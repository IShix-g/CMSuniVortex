
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using System.IO;
using UnityEditor;
#endif

namespace Test
{
    public sealed class SceneSelector : MonoBehaviour
    {
        [SerializeField] string[] _scenesNames;

        public string[] ScenesNames => _scenesNames;

        public void LoadScene(string sceneName)
            => SceneManager.LoadScene(sceneName);

        public void LoadScene(int sceneIndex)
            => SceneManager.LoadScene(_scenesNames[sceneIndex]);

        void Reset()
        {
#if UNITY_EDITOR
            _scenesNames = EditorBuildSettings.scenes
                .Where(scene => scene.enabled)
                .Select(scene => Path.GetFileNameWithoutExtension(scene.path))
                .ToArray();
#endif
        }
    }
}