
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
        [SerializeReference] ICuvOutput _output;
        [SerializeField] string[] _modelListGuilds;
        
        public bool IsBuildCompleted => _modelListGuilds.Length > 0;
        public SystemLanguage[] Languages => _languages;
        public string[] ModelListGuilds => _modelListGuilds;
        public bool IsLoading { get; private set; }

        #region Editor
        protected void SetBuildPath(string buildPath) => _buildPath = buildPath;
        
        protected void SetClient(ICuvClient client) => _client = client;
        
        protected void SetOutput(ICuvOutput output) => _output = output;
        
#if UNITY_EDITOR
        bool ICuvImporter.CanIImport()
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
        
        void ICuvImporter.StartImport(Action onLoaded)
        {
            if (IsLoading)
            {
                return;
            }
            IsLoading = true;
            
            Debug.Log("Start importing.");
            EditorCoroutineUtility.StartCoroutine(_client.Load(_buildPath, _languages, guids =>
            {
                EditorUtility.SetDirty(this);
                AssetDatabase.SaveAssetIfDirty(this);
                
                IsLoading = false;
                _modelListGuilds = guids;
                if (CanIOutput())
                {
                    StartOutput();
                }
                onLoaded?.Invoke();
                Debug.Log("Completion of importing.");
                
            }), this);
        }

        bool ICuvImporter.CanIOutput() => CanIOutput();

        void ICuvImporter.StartOutput() => StartOutput();

        bool CanIOutput() => _client != default
                             && _modelListGuilds is {Length: > 0}
                             && _output != default;

        void StartOutput()
        {
            if (!IsLoading)
            {
                _output.Generate(_buildPath, _client, _modelListGuilds);
                EditorUtility.SetDirty(this);
                AssetDatabase.SaveAssetIfDirty(this);
            }
        }
        
        void ICuvImporter.SelectClient()
        {
            IsLoading = true;
            EditorApplication.delayCall += () =>
            {
                _client.Select(_buildPath);
                IsLoading = false;
            };
        }

        void ICuvImporter.DeselectClient() => _client?.Deselect();
        
        void ICuvImporter.SelectOutput()
        {
            IsLoading = true;
            EditorApplication.delayCall += () =>
            {
                _output.Select(_buildPath);
                IsLoading = false;
            };
        }

        void ICuvImporter.DeselectOutput() => _output?.Deselect();

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
        bool ICuvImporter.CanIImport() => false;
        void ICuvImporter.StartImport(Action onLoaded) => throw new NotImplementedException();
        bool ICuvImporter.CanIOutput() => false;
        void ICuvImporter.StartOutput() => throw new NotImplementedException();
        void ICuvImporter.SelectOutput() => throw new NotImplementedException();
        void ICuvImporter.DeselectOutput() => throw new NotImplementedException();
#endif

        #endregion
    }
}