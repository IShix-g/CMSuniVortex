
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using JetBrains.Annotations;
using Unity.EditorCoroutines.Editor;
using Debug = UnityEngine.Debug;

#if ENABLE_ADDRESSABLES
using CMSuniVortex.Addressable;
#endif

namespace CMSuniVortex.Editor
{
    [CustomEditor(typeof(CuvImporter), true), CanEditMultipleObjects]
    sealed class CuvImporterEditor : UnityEditor.Editor
    {
        const string _packageUrl = "https://raw.githubusercontent.com/IShix-g/CMSuniVortex/main/Packages/CMSuniVortex/package.json";
        const string _packagePath = "Packages/com.ishix.cmsunivortex/";
        
        SerializedProperty _scriptProp;
        SerializedProperty _buildPathProp;
        SerializedProperty _languagesProp;
        SerializedProperty _clientProp;
        SerializedProperty _outputProp;
        CuvTypePopup _clientTypePopup;
        CuvTypePopup _outputTypePopup;
        ICuvImporter _importer;
        ICuvDoc _cuvDoc;
        Texture2D _logo;
        Texture2D _importIcon;
        Texture2D _outputIcon;
        string _currentVersion;
        bool _isStartCheckVersion;

#if ENABLE_ADDRESSABLES
        IAddressableSettingsProvider _clientAddressableSettingsProvider;
        IAddressableSettingsProvider _outputAddressableSettingsProvider;
#endif

        static readonly string[] s_propertiesToExclude = {"m_Script", "_buildPath", "_languages", "_client", "_output", "_modelListGuilds"};
        
        void OnEnable()
        {
            _scriptProp = serializedObject.FindProperty(s_propertiesToExclude[0]);
            _buildPathProp = serializedObject.FindProperty(s_propertiesToExclude[1]);
            _languagesProp = serializedObject.FindProperty(s_propertiesToExclude[2]);
            _clientProp = serializedObject.FindProperty(s_propertiesToExclude[3]);
            _outputProp = serializedObject.FindProperty(s_propertiesToExclude[4]);
            {
                var types = TypeCache.GetTypesDerivedFrom<ICuvClient>()
                    .Where(type => typeof(ICuvClient).IsAssignableFrom(type)
                                && !type.IsInterface
                                && !type.IsAbstract
                                && !type.GetCustomAttributes(typeof(CuvIgnoreAttribute), false).Any())
                    .ToArray();
                _clientTypePopup = new CuvTypePopup(_clientProp, types);
            }
            _outputTypePopup = new CuvTypePopup(_outputProp, GetFilteredOutputTypes());
            
            _importer = target as ICuvImporter;
            _logo = GetLogo();
            _importIcon = GetImportIcon();
            _outputIcon = GetOutputIcon();
            _currentVersion = "v" + CheckVersion.GetCurrent(_packagePath);
            _isStartCheckVersion = false;
            _clientProp.isExpanded = true;
            _cuvDoc = _clientProp.managedReferenceValue as ICuvDoc;
            _outputProp.isExpanded = true;
            
            SetClientAddressableSettingsProvider();
            SetOutputAddressableSettingsProvider();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.UpdateIfRequiredOrScript();
            
            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.PropertyField(_scriptProp, true);
            }
            
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Script Generator"))
            {
                ScriptGeneratorWindow.ShowDialog();
            }
            if (GUILayout.Button("Github"))
            {
                Application.OpenURL("https://github.com/IShix-g/CMSuniVortex");
            }
            EditorGUI.BeginDisabledGroup(_isStartCheckVersion);
            if (GUILayout.Button("Check for Update"))
            {
                _isStartCheckVersion = true;
                EditorCoroutineUtility.StartCoroutine(
                    CheckVersion.GetVersionOnServer(
                        _packageUrl,
                        version =>
                        {
                            var comparisonResult = 0;
                            if (!string.IsNullOrEmpty(version))
                            {
                                Debug.Log("Local: " + _currentVersion + " | GitHub: v" + version);
                                var current = new Version(_currentVersion.TrimStart('v').Trim());
                                var server = new Version(version.Trim());
                                comparisonResult = current.CompareTo(server);
                                version = "v" + version;
                            }
                            
                            if (comparisonResult >= 0)
                            {
                                EditorUtility.DisplayDialog("Check for Update", "The current version is the latest release.", "Close");
                            }
                            else
                            {
                                EditorUtility.DisplayDialog(_currentVersion + " -> " + version, "There is a newer version (" + version + "), please update from Package Manager.", "Close");
                            }
                            _isStartCheckVersion = false;
                        },
                        () => _isStartCheckVersion = false), this);
            }
            EditorGUI.EndDisabledGroup();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            
            GUILayout.BeginVertical(GUI.skin.box);
            {
                var style = new GUIStyle(GUI.skin.label)
                {
                    padding = new RectOffset(5, 5, 5, 5),
                    alignment = TextAnchor.MiddleCenter,
                };
                GUILayout.Label(_logo, style, GUILayout.MinWidth(430), GUILayout.Height(75));
            }
            {
                var style = new GUIStyle(GUI.skin.label)
                {
                    alignment = TextAnchor.MiddleCenter,
                };
                EditorGUILayout.LabelField(_currentVersion, style);
            }
            GUILayout.EndVertical();
            
            GUILayout.Space(10);
            
            GUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(_buildPathProp);
            if (GUILayout.Button("Select", GUILayout.Width(60)))
            {
                if (!AssetDatabase.IsValidFolder(_buildPathProp.stringValue))
                {
                    _buildPathProp.stringValue = AssetDatabase.GetAssetPath(target);
                }
                
                _buildPathProp.stringValue = EditorUtility.OpenFolderPanel(
                    "Select Build Path",
                    string.IsNullOrEmpty(_buildPathProp.stringValue)
                        ? "Assets/"
                        : _buildPathProp.stringValue, "Select Build Path");
                if (!string.IsNullOrEmpty(_buildPathProp.stringValue))
                {
                    var assetsIndex = _buildPathProp.stringValue.IndexOf("Assets", StringComparison.Ordinal);
                    if (assetsIndex >= 0)
                    {
                        _buildPathProp.stringValue = _buildPathProp.stringValue.Substring(assetsIndex);
                        if (!string.IsNullOrEmpty(_buildPathProp.stringValue)
                            && !_buildPathProp.stringValue.EndsWith('/'))
                        {
                            _buildPathProp.stringValue += "/";
                        }
                    }
                    else
                    {
                        _buildPathProp.stringValue = string.Empty;
                        Debug.LogError("Please select the build path under Assets.");
                    }
                }
            }
            GUILayout.EndHorizontal();
            
            GUILayout.Space(5);
            
            EditorGUILayout.PropertyField(_languagesProp);
            
            GUILayout.Space(10);
            
            var prop = serializedObject.GetIterator();
            if (prop.NextVisible(true))
            {
                do
                {
                    if (!s_propertiesToExclude.Contains(prop.name)) 
                    {
                        EditorGUILayout.PropertyField(prop, true);
                    }
                }
                while (prop.NextVisible(false));
            }
            
            GUILayout.Space(10);

            _cuvDoc ??= _clientProp.managedReferenceValue as ICuvDoc;
            
            if (_cuvDoc != default)
            {
                var boxStyle = new GUIStyle(GUI.skin.box)
                {
                    padding = new RectOffset(5, 5, 5, 5)
                };
                GUILayout.BeginHorizontal(boxStyle);
                var labelStyle = new GUIStyle(GUI.skin.label)
                {
                    padding = new RectOffset(5, 5, 5, 5),
                    alignment = TextAnchor.MiddleCenter,
                };
                GUILayout.Label(_cuvDoc.GetCmsName(), labelStyle);
                if (!string.IsNullOrEmpty(_cuvDoc.GetDocUrl())
                    && GUILayout.Button("doc", GUILayout.Width(50)))
                {
                    Application.OpenURL(_cuvDoc.GetDocUrl());
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(10);
            }
            
            if (_clientTypePopup.Draw())
            {
                SetClientAddressableSettingsProvider();
                _outputTypePopup.ResetTypes(GetFilteredOutputTypes());
                _outputTypePopup.ResetReference();

                if (_clientProp.managedReferenceValue != default)
                {
                    _importer.SelectClient();
                }
                else
                {
                    _importer.DeselectClient();
                }
            }

            GUILayout.Space(20);
            
            EditorGUI.BeginDisabledGroup(_importer.IsLoading);
            {
                var content = new GUIContent(_importer.IsLoading ? " Now importing..." : " Import", _importIcon);
                if (GUILayout.Button(content, GUILayout.Height(38)))
                {
                    if (_importer.CanIImport())
                    {
                        _importer.StartImport();
                    }
                }
            }
            EditorGUI.EndDisabledGroup();
            
            GUILayout.Space(10);

            if (_outputTypePopup.Draw())
            {
                SetOutputAddressableSettingsProvider();
                
                if (_outputTypePopup.Property.managedReferenceValue != default)
                {
                    _importer.SelectOutput();
                }
                else
                {
                    _importer.DeselectOutput();
                }
            }
            
#if ENABLE_ADDRESSABLES
            if (_clientAddressableSettingsProvider != default
                && _outputAddressableSettingsProvider != default)
            {
                var setting = _clientAddressableSettingsProvider.GetSetting();
                _outputAddressableSettingsProvider.SetSetting(setting);
            }
#endif
            
            GUILayout.Space(10);
            
            EditorGUI.BeginDisabledGroup(!_importer.CanIOutput() || _importer.IsLoading);
            {
                var content = new GUIContent("Output", _outputIcon);
                if (GUILayout.Button(content, GUILayout.Height(38)))
                {
                    if (_importer.CanIOutput())
                    {
                        _importer.StartOutput();
                    }
                }
            }

            EditorGUI.EndDisabledGroup();
            
            serializedObject.ApplyModifiedProperties();
        }

        [CanBeNull]
        internal static Texture2D GetLogo() => GetTexture("CuvDataImporterLogo");
        
        [CanBeNull]
        internal static Texture2D GetImportIcon() => GetTexture("CuvImportIcon");
        
        internal static Texture2D GetOutputIcon() => GetTexture("CuvOutputIcon");
        
        [CanBeNull]
        static Texture2D GetTexture(string textureName)
        {
            var guids = AssetDatabase.FindAssets("t:Texture2D " + textureName);
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            }
            return default;
        }
        
        Type[] GetFilteredOutputTypes()
        {
            if (_clientProp.managedReferenceValue == default)
            {
                return Array.Empty<Type>();
            }
            var clientTypes = ExtractModelAndListTypes(_clientProp.managedReferenceValue.GetType());
            if (clientTypes.ModelType == default
                || clientTypes.ListType == default)
            {
                return Array.Empty<Type>();
            }
            return _clientProp.managedReferenceValue != default ?
                TypeCache.GetTypesDerivedFrom<ICuvOutput>()
                    .Where(type => typeof(ICuvOutput).IsAssignableFrom(type)
                                   && !type.IsInterface
                                   && !type.IsAbstract
                                   && !type.GetCustomAttributes(typeof(CuvIgnoreAttribute), false).Any()
                                   && IsTypeMatch(type, clientTypes.ModelType, clientTypes.ListType))
                    .ToArray()
                : Array.Empty<Type>();
        }
        
        bool IsTypeMatch(Type targetType, Type modelType, Type listType)
        {
            if (modelType == default
                || listType == default)
            {
                return false;
            }
            var types = ExtractModelAndListTypes(targetType);
            return modelType == types.ModelType
                   && listType == types.ListType;
        }
        
        (Type ModelType, Type ListType) ExtractModelAndListTypes(Type objType)
        {
            if (objType == default)
            {
                return (default, default);
            }
            if (objType.IsConstructedGenericType)
            {
                return ExtractTypeArguments(objType);
            }
            if (objType.BaseType is {IsConstructedGenericType: true})
            {
                return ExtractModelAndListTypes(objType.BaseType);
            }

            var genericInterface = objType.GetInterfaces()
                    .FirstOrDefault(type => type.IsConstructedGenericType
                                            && type.GetInterfaces().Contains(typeof(ICuvOutput)));
            return genericInterface != default
                ? ExtractTypeArguments(genericInterface)
                : (default, default);
        }

        (Type ModelType, Type ListType) ExtractTypeArguments(Type type)
        {
            var typeArguments = type.GetGenericArguments();
            var modelType = typeArguments.Length > 0 ? typeArguments[0] : default;
            var modelListType = typeArguments.Length > 1 ? typeArguments[1] : default;
            return (modelType, modelListType);
        }
        
        void SetClientAddressableSettingsProvider()
        {
#if ENABLE_ADDRESSABLES
            var providerType = _clientProp.managedReferenceValue?.GetType().GetInterfaces()
                .FirstOrDefault(type => type == typeof(IAddressableSettingsProvider));
            if (providerType != default
                && providerType.IsInstanceOfType(_clientProp.managedReferenceValue))
            {
                _clientAddressableSettingsProvider = (IAddressableSettingsProvider) _clientProp.managedReferenceValue;
            }
#endif
        }
        
        void SetOutputAddressableSettingsProvider()
        {
#if ENABLE_ADDRESSABLES
            var providerType = _outputProp.managedReferenceValue?.GetType().GetInterfaces()
                .FirstOrDefault(type => type == typeof(IAddressableSettingsProvider));
            if (providerType != default
                && providerType.IsInstanceOfType(_outputProp.managedReferenceValue))
            {
                _outputAddressableSettingsProvider = (IAddressableSettingsProvider) _outputProp.managedReferenceValue;
            }
#endif
        }
    }
}