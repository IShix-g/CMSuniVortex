
using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using JetBrains.Annotations;
using Unity.EditorCoroutines.Editor;

#if ENABLE_ADDRESSABLES
using CMSuniVortex.Addressable;
#endif

namespace CMSuniVortex.Editor
{
    [CustomEditor(typeof(CuvImporter), true), CanEditMultipleObjects]
    internal sealed class CuvImporterEditor : UnityEditor.Editor
    {
        const string _packageUrl = "https://raw.githubusercontent.com/IShix-g/CMSuniVortex/main/Packages/CMSuniVortex/package.json";
        const string _packagePath = "Packages/com.ishix.cmsunivortex/";
        const string _packageName = "com.ishix.cmsunivortex";
        const string _gitUrl = "https://github.com/IShix-g/CMSuniVortex";
        const string _gitInstallUrl = _gitUrl + ".git?path=Packages/CMSuniVortex";
        static readonly string[] s_propertiesToExclude = {"m_Script", "_buildPath", "_client", "_output", "_modelListGuilds"};
        
        enum UpdateFlag { None, Update, Error, NowLoading }
        
        SerializedProperty _scriptProp;
        SerializedProperty _buildPathProp;
        SerializedProperty _clientProp;
        SerializedProperty _outputProp;
        CuvTypePopup _clientTypePopup;
        CuvTypePopup _outputTypePopup;
        ICuvImporter _importer;
        Texture2D _logo;
        Texture2D _importIcon;
        Texture2D _outputIcon;
        string _currentVersion;
        string _updateText = "---";
        UpdateFlag _updateFlag;
        bool _isProcessing;
        readonly PackageInstaller _packageInstaller = new ();
        CuvImporterAttribute _importerSetting;
        
#if ENABLE_ADDRESSABLES
        IAddressableSettingsProvider _clientAddressableSettingsProvider;
        IAddressableSettingsProvider _outputAddressableSettingsProvider;
#endif
        
        void OnEnable()
        {
            _importerSetting = target.GetType().GetCustomAttribute<CuvImporterAttribute>() ?? new CuvImporterAttribute();
            _scriptProp = serializedObject.FindProperty(s_propertiesToExclude[0]);
            _buildPathProp = serializedObject.FindProperty(s_propertiesToExclude[1]);
            _clientProp = serializedObject.FindProperty(s_propertiesToExclude[2]);
            _outputProp = serializedObject.FindProperty(s_propertiesToExclude[3]);
            {
                var types = TypeCache.GetTypesDerivedFrom<ICuvClient>()
                    .Where(type => typeof(ICuvClient).IsAssignableFrom(type)
                                && !type.IsInterface
                                && !type.IsAbstract
                                && !type.GetCustomAttributes(typeof(CuvIgnoreAttribute), false).Any())
                    .ToArray();
                _clientTypePopup = new CuvTypePopup(_clientProp, types)
                {
                    IsEnabledSelect = _importerSetting.IsEnabledSelectClient
                };
            }
            _outputTypePopup = new CuvTypePopup(_outputProp, GetFilteredOutputTypes())
            {
                IsEnabledSelect = _importerSetting.IsEnabledSelectOutput
            };

            _importer = target as ICuvImporter;
            _logo = GetLogo();
            _importIcon = GetImportIcon();
            _outputIcon = GetOutputIcon();
            _currentVersion = "v" + CheckVersion.GetCurrent(_packagePath);
            _clientProp.isExpanded = true;
            _outputProp.isExpanded = true;
            
            SetClientAddressableSettingsProvider();
            SetOutputAddressableSettingsProvider();
        }

        void OnDisable()
        {
            if (_packageInstaller.IsProcessing)
            {
                _packageInstaller.Cancel();
            }
        }
        
        public override void OnInspectorGUI()
        {
            serializedObject.UpdateIfRequiredOrScript();
            
            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.PropertyField(_scriptProp, true);
            }

            GUILayout.Space(10);
            
            if (_importerSetting.IsShowMenu)
            {
                ShowMenu();
                GUILayout.Space(10);
            }

            if (_importerSetting.IsShowLogo)
            {
                ShowLogo();
                GUILayout.Space(10);
            }

            EditorGUI.BeginDisabledGroup(
                _isProcessing
                || _updateFlag == UpdateFlag.NowLoading
                || _importer.IsLoading);
            
            GUILayout.BeginHorizontal();
            EditorGUI.BeginDisabledGroup(!_importerSetting.IsEnabledBuildPath);
            EditorGUILayout.PropertyField(_buildPathProp);
            
            var buttonClicked = GUILayout.Button("Select", GUILayout.Width(60));
            GUILayout.EndHorizontal();

            if (buttonClicked)
            {
                if (!AssetDatabase.IsValidFolder(_buildPathProp.stringValue))
                {
                    _buildPathProp.stringValue = AssetDatabase.GetAssetPath(target);
                }
                
                var selectedPath = EditorUtility.OpenFolderPanel(
                    "Select Build Path",
                    string.IsNullOrEmpty(_buildPathProp.stringValue)
                        ? "Assets/"
                        : _buildPathProp.stringValue, "Select Build Path");
                
                if (!string.IsNullOrEmpty(selectedPath))
                {
                    _buildPathProp.stringValue = selectedPath;
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
            
            EditorGUI.EndDisabledGroup();
            
            GUILayout.Space(5);
            
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
            
            if (_clientProp.managedReferenceValue is ICuvDoc cuvDoc)
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
                GUILayout.Label(cuvDoc.GetCmsName(), labelStyle);
                if (!string.IsNullOrEmpty(cuvDoc.GetDocUrl())
                    && GUILayout.Button("doc", GUILayout.Width(50)))
                {
                    Application.OpenURL(cuvDoc.GetDocUrl());
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(10);
            }
            
            var prev = _clientProp.managedReferenceValue;
            if (_clientTypePopup.Draw())
            {
                SetClientAddressableSettingsProvider();
                _outputTypePopup.ResetTypes(GetFilteredOutputTypes());
                _outputTypePopup.ResetReference();
                
                if (prev != null)
                {
                    _importer.DeselectClient();
                }
                if (_clientProp.managedReferenceValue != default)
                {
                    _importer.SelectClient();
                }

                _updateFlag = UpdateFlag.None;
                _updateText = "---";
            }

            if (_clientProp.managedReferenceValue is ICuvUpdateChecker checker)
            {
                GUILayout.Space(5);
                var boxStyle = new GUIStyle(GUI.skin.box)
                {
                    padding = new RectOffset(5, 5, 5, 5),
                };
                GUILayout.BeginHorizontal(boxStyle);

                EditorGUI.BeginDisabledGroup(!checker.IsUpdateAvailable());
                
                if (GUILayout.Button("Check for Updates", GUILayout.Width(130)))
                {
                    _updateFlag = UpdateFlag.NowLoading;
                    _updateText = "Now check for updates...";
                    
                    checker.CheckForUpdate(_buildPathProp.stringValue, (has, msg) =>
                    {
                        _updateText = has ? "Updates available. Please import.\n" : "You have the latest version.\n";
                        _updateText += msg;
                        _updateFlag = has ? UpdateFlag.Update : UpdateFlag.None;
                    }, msg =>
                    {
                        _updateText = "Update check failed.\n";
                        _updateText += msg;
                        _updateFlag = UpdateFlag.Error;
                    });
                }
                EditorGUI.EndDisabledGroup();

                var labelStyle = new GUIStyle(GUI.skin.label)
                {
                    padding = new RectOffset(5, 5, 5, 5),
                    alignment = TextAnchor.MiddleLeft
                };
                labelStyle.normal.textColor = _updateFlag switch
                {
                    UpdateFlag.NowLoading => Color.green,
                    UpdateFlag.Update => Color.cyan,
                    UpdateFlag.Error => Color.red,
                    _ => labelStyle.normal.textColor
                };
                GUILayout.Label(_updateText, labelStyle);
                GUILayout.EndHorizontal();
            }
            
            GUILayout.Space(20);
            
            using (new EditorGUI.DisabledScope(!_importerSetting.IsEnabledImportButton))
            {
                var content = new GUIContent(_importer.IsLoading ? " Now importing..." : " Import", _importIcon);
                if (GUILayout.Button(content, GUILayout.Height(38)))
                {
                    if (_importer.CanIImport())
                    {
                        _importer.StartImport();
                        _updateFlag = UpdateFlag.None;
                        _updateText = "---";
                    }
                }
            }
            
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
            
            using (new EditorGUI.DisabledScope(!_importer.CanIOutput() || !_importerSetting.IsEnabledOutputButton))
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

            if (!_importerSetting.IsShowLogo)
            {
                GUILayout.Space(10);
                ShowAppText();
            }

            EditorGUI.EndDisabledGroup();
            
            serializedObject.ApplyModifiedProperties();
        }

        void ShowMenu()
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Script Generator"))
            {
                ScriptGeneratorWindow.ShowDialog();
            }
            if (GUILayout.Button("Github"))
            {
                Application.OpenURL(_gitUrl);
            }

            if (GUILayout.Button("Check for Update"))
            {
                EditorCoroutineUtility.StartCoroutine(
                    CheckVersion.GetVersionOnServer(
                        _packageUrl,
                        version =>
                        {
                            var comparisonResult = 0;
                            if (!string.IsNullOrEmpty(version))
                            {
                                var current = new Version(_currentVersion.TrimStart('v').Trim());
                                var server = new Version(version.Trim());
                                comparisonResult = current.CompareTo(server);
                                version = "v" + version;
                            }
                            
                            if (comparisonResult >= 0)
                            {
                                EditorUtility.DisplayDialog(_currentVersion + "\nYou're up to date.", "You have the latest version installed.", "OK");
                            }
                            else
                            {
                                var isOpen = EditorUtility.DisplayDialog(_currentVersion + " -> " + version, "There is a newer version " + version + ".", "Update", "Close");
                                
                                if (isOpen)
                                {
                                    _packageInstaller.Install(new []{ _gitInstallUrl }).Handled();
                                }
                            }
                        }), this);
            }
            GUILayout.EndHorizontal();
        }

        void ShowLogo()
        {
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
        }

        void ShowAppText()
        {
            {
                var style = new GUIStyle(GUI.skin.label)
                {
                    alignment = TextAnchor.MiddleRight,
                    padding = new RectOffset(5, 5, 5, 5),
                    fontSize = 11,
                };
                EditorGUILayout.LabelField("CMSuniVortex " + _currentVersion, style);
            }
        }
        
        [CanBeNull]
        internal static Texture2D GetLogo() => GetTexture("CuvDataImporterLogo");
        
        [CanBeNull]
        internal static Texture2D GetImportIcon() => GetTexture("CuvImportIcon");
        
        internal static Texture2D GetOutputIcon() => GetTexture("CuvOutputIcon");
        
        [CanBeNull]
        public static Texture2D GetTexture(string textureName)
        {
            return AssetDatabase.LoadAssetAtPath<Texture2D>(_packagePath + "Editor/Textures/" + textureName + ".png");
        }
        
        Type[] GetFilteredOutputTypes()
        {
            if (_clientProp.managedReferenceValue == default)
            {
                return Array.Empty<Type>();
            }

            var reference = _clientProp.managedReferenceValue.GetType();
            var clientTypes = ExtractModelAndListTypes(reference);
            if (clientTypes.ModelType == default
                || clientTypes.ListType == default)
            {
                return Array.Empty<Type>();
            }
            return _clientProp.managedReferenceValue != default 
                ? TypeCache.GetTypesDerivedFrom<ICuvOutput>()
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