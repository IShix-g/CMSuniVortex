
using System;
using System.Collections.Generic;
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
    [HelpURL("https://github.com/IShix-g/CMSuniVortex?tab=readme-ov-file")]
    public class CuvImporter : ScriptableObject, ICuvImporter, ICuvImporterStatus
    {
        [SerializeField] string _buildPath;
        [SerializeField] SystemLanguage[] _languages;
        [SerializeReference] ICuvClient _client;
        [SerializeReference] ICuvOutput _output;
        [SerializeField] string[] _modelListGuilds;
        ICuvImporterStatus _cuvImporterStatusImplementation;

        public bool IsBuildCompleted => _modelListGuilds.Length > 0
                                        && _output != default
                                        && _output.IsCompleted();
        public string BuildPath => _buildPath;
        public SystemLanguage[] Languages => _languages;
        public ICuvClient Client
        {
            get => _client;
            set
            {
                _client = value;
#if UNITY_EDITOR
                EditorUtility.SetDirty(this);
                AssetDatabase.SaveAssetIfDirty(this);
#endif
            }
        }
        public ICuvOutput Output
        {
            get => _output;
            set
            {
                _output = value;
#if UNITY_EDITOR
                EditorUtility.SetDirty(this);
                AssetDatabase.SaveAssetIfDirty(this);
#endif
            }
        }
        public string[] ModelListGuilds => _modelListGuilds;
        public bool IsLoading { get; private set; }
        
        protected void SetBuildPath(string buildPath) => _buildPath = buildPath;

        protected virtual void OnStartImport(string buildPath, IReadOnlyList<SystemLanguage> languages){}
        protected virtual void OnImported(string[] listGuids){}
        protected virtual void OnStartOutput(string buildPath, ICuvClient client, ICuvOutput output, string[] listGuids){}
        protected virtual void OnOutputted(ICuvOutput output, string[] listGuids){}

        protected virtual void OnEnable()
        {
#if UNITY_EDITOR
            if( EditorApplication.isPlayingOrWillChangePlaymode )
            {
                EditorApplication.playModeStateChanged += LogPlayModeState;
            }
#endif  
        }

#if UNITY_EDITOR
        void LogPlayModeState(PlayModeStateChange state)
        {
            if (state != PlayModeStateChange.ExitingPlayMode)
            {
                return;
            }
            EditorApplication.playModeStateChanged -= LogPlayModeState;
            EditorApplication.delayCall += () =>
            {
                if (_output != default
                    && !string.IsNullOrEmpty(_buildPath))
                {
                    _output.ReloadReference(_buildPath);
                }
            };
        }

        string ICuvImporterStatus.GetName() => name;

        string ICuvImporterStatus.GetClientName()
        {
#if UNITY_EDITOR
            if (_client != default)
            {
                var type = _client.GetType();
                var customAttributes = type.GetCustomAttributes(typeof(CuvClientAttribute), true);
                if (customAttributes.Length > 0)
                {
                    var attribute = (CuvClientAttribute)customAttributes[0];
                    return attribute.ClientName;
                }
            }
#endif
            return string.Empty;
        }
        
        string ICuvImporterStatus.GetClintClassName() => _client != default ? _client.GetType().Name : string.Empty;

        string ICuvImporterStatus.GetOutputClassName()=> _output != default ? _output.GetType().Name : string.Empty;

        string ICuvImporterStatus.GetBuildPath() => _buildPath;
        
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
            OnStartImport(_buildPath, _languages);
            EditorCoroutineUtility.StartCoroutine(_client.Load(_buildPath, _languages, listGuilds =>
            {
                OnImported(listGuilds);
                EditorUtility.SetDirty(this);
                AssetDatabase.SaveAssetIfDirty(this);
                Debug.Log("Completion of importing.");
                
                IsLoading = false;
                _modelListGuilds = listGuilds;
                if (CanIOutput())
                {
                    StartOutput();
                }
                onLoaded?.Invoke();
            }), this);
        }

        bool ICuvImporter.CanIOutput() => CanIOutput();

        void ICuvImporter.StartOutput() => StartOutput();

        bool CanIOutput() => _client != default
                             && _modelListGuilds is {Length: > 0}
                             && _output != default;

        void StartOutput()
        {
            if (IsLoading)
            {
                return;
            }
            Debug.Log("Start Output.");
            OnStartOutput(_buildPath, _client, _output, _modelListGuilds);
            _output.Generate(_buildPath, _client, _modelListGuilds);
            OnOutputted(_output, _modelListGuilds);
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssetIfDirty(this);
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

        bool IsFileOrDirectoryExists(string path)
            => Directory.Exists(path) || File.Exists(path);
#else
        bool ICuvImporter.CanIImport() => false;
        void ICuvImporter.StartImport(Action onLoaded) => throw new NotImplementedException();
        void ICuvImporter.SelectClient() => throw new NotImplementedException();
        void ICuvImporter.DeselectClient() => throw new NotImplementedException();
        bool ICuvImporter.CanIOutput() => false;
        void ICuvImporter.StartOutput() => throw new NotImplementedException();
        void ICuvImporter.SelectOutput() => throw new NotImplementedException();
        void ICuvImporter.DeselectOutput() => throw new NotImplementedException();
#endif
        
        protected virtual void Reset()
        {
#if UNITY_EDITOR
            _languages = new[] { SystemLanguage.English };
            if (Selection.activeObject != default)
            {
                var path = AssetDatabase.GetAssetPath(Selection.activeObject);
                if (!string.IsNullOrEmpty(path))
                {
                    _buildPath = Path.GetDirectoryName(path);
                    if (!string.IsNullOrEmpty(_buildPath)
                        && !_buildPath.EndsWith('/'))
                    {
                        _buildPath += "/";
                    }
                }
            }
#endif
        }
    }
}