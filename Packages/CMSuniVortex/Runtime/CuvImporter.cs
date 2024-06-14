
using System;
using System.IO;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using Unity.EditorCoroutines.Editor;
#endif

namespace CMSuniVortex
{
    /// <summary>
    /// CuvImporter is responsible for importing CMS data into Unity.
    /// </summary>
    [CreateAssetMenu(menuName = "CMSuniVortex/create CuvImporter", fileName = "New CuvImporter", order = 0)]
    public class CuvImporter : ScriptableObject, ICuvImporter
    {
        [SerializeField] string _buildPath;
        [SerializeField] SystemLanguage[] _languages;
        [SerializeReference] ICuvClient _client;
        [SerializeField] string[] _modelListGuilds;
        
        public bool IsBuildCompleted => _modelListGuilds.Length > 0;
        public SystemLanguage[] Languages => _languages;
        public string[] ModelListGuilds => _modelListGuilds;
        public bool IsLoading { get; private set; }

        #region Editor
        protected void SetBuildPath(string buildPath) => _buildPath = buildPath;
        
        protected void SetClient(ICuvClient client) => _client = client;
        
#if UNITY_EDITOR
        public bool CanImport()
        {
            if (string.IsNullOrEmpty(_buildPath))
            {
                Debug.LogError("Please input the BuildPath.");
                return false;
            }
            if (!IsFileOrDirectoryExists(_buildPath))
            {
                Debug.LogError("BuildPath does not exist.");
                return false;
            }
            if (_languages == default
                || _languages.Length == 0)
            {
                Debug.LogError("Please set one or more items in Languages.");
                return false;
            }
            if (_client == default)
            {
                Debug.LogError("Please set Client.");
                return false;
            }
            if (!_client.CanILoad())
            {
                return false;
            }
            return true;
        }
        
        public void StartImport(Action onLoaded = default)
        {
            if (IsLoading)
            {
                return;
            }
            IsLoading = true;
            
            Debug.Log("Start importing.");
            EditorCoroutineUtility.StartCoroutine(_client.Load(_buildPath, _languages, guids =>
            {
                _modelListGuilds = guids;
                AssetDatabase.SaveAssetIfDirty(this);
                onLoaded?.Invoke();
                Debug.Log("Completion of importing.");
                IsLoading = false;
            }), this);
        }
        
        protected virtual void Reset()
        {
            _languages = new[] { SystemLanguage.English };
            var path = AssetDatabase.GetAssetPath(Selection.activeObject);
            _buildPath = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(_buildPath)
                && !_buildPath.EndsWith('/'))
            {
                _buildPath += "/";
            }
        }

        bool IsFileOrDirectoryExists(string path)
            => Directory.Exists(path) || File.Exists(path);
#else
        public bool CanImport() => false;
        public void StartImport(Action onLoaded = default) => onLoaded?.Invoke();
        public bool CanILoad() => false;
        public void StartLoad(Action onLoaded = default){}
#endif
        #endregion
    }
}