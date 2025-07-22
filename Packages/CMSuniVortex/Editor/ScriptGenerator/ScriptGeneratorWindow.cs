
using System;
using UnityEditor;
using UnityEngine;

namespace CMSuniVortex.Editor
{
    internal sealed class ScriptGeneratorWindow : EditorWindow
    {
        [MenuItem("Window/CMSuniVortex/open Script Generator", false, 2)]
        public static void ShowDialogInternal() => ShowDialog();

        public static ScriptGeneratorWindow ShowDialog()
        {
            var window = GetWindow<ScriptGeneratorWindow>("Script Generator");
            window.Show();
            window.minSize = new Vector2(480, 480);
            return window;
        }

        public static void ShowDialog(string className, string buildPath = default)
        {
            var window = ShowDialog();
            if (string.IsNullOrEmpty(buildPath)
                || window._className != className)
            {
                window._buildPath = buildPath;
            }
            window._className = className;
        }
        
        Texture2D _logo;
        string _className;
        string _buildPath;
        bool _isGenerateAddressableClient;
        bool _enableAddressables;
        bool _useAddressables;
        bool _useLocalization;
        bool _isGenerateOutput = true;
        
        void OnEnable()
        {
            _logo = CuvImporterEditor.GetLogo();
            
            #if ENABLE_ADDRESSABLES
            _enableAddressables = _useAddressables = true;
            #endif
        }
        
        void OnGUI()
        {
            {
                GUILayout.BeginVertical(GUI.skin.box);
                {
                    var style = new GUIStyle(GUI.skin.label)
                    {
                        padding = new RectOffset(5, 5, 5, 5),
                        alignment = TextAnchor.MiddleCenter,
                    };
                    GUILayout.Label(_logo, style, GUILayout.MinWidth(50), GUILayout.MaxHeight(80));
                }

                {
                    var style = new GUIStyle(GUI.skin.label)
                    {
                        fontSize = 14,
                        padding = new RectOffset(0, 0, 10, 10),
                        alignment = TextAnchor.MiddleCenter,
                        wordWrap = true
                    };
                    GUILayout.Label("Generates the required class from the class name.", style);
                }
                GUILayout.EndVertical();
            }
            
            GUILayout.BeginVertical(new GUIStyle() { padding = new RectOffset(10, 10, 10, 10) });
            {
                _className = EditorGUILayout.TextField("Full Class Name", _className);
                GUILayout.Space(5);
                
                GUILayout.BeginHorizontal();
                _buildPath = EditorGUILayout.TextField("Build Path", _buildPath);
                if (GUILayout.Button("Select", GUILayout.Width(60)))
                {
                    var path = EditorUtility.OpenFolderPanel("Select Build Path", "Assets/", "");
                    if (!string.IsNullOrEmpty(path))
                    {
                        _buildPath = path;
                        var assetsIndex = _buildPath.IndexOf("Assets", StringComparison.Ordinal);
                        if (assetsIndex >= 0)
                        {
                            _buildPath = _buildPath.Substring(assetsIndex);
                            if (!string.IsNullOrEmpty(_buildPath)
                                && !_buildPath.EndsWith('/'))
                            {
                                _buildPath += "/";
                            }
                        }
                        else
                        {
                            _buildPath = "Assets/";
                            Debug.LogError("Please select the build path under Assets.");
                        }
                    }
                }
                GUILayout.EndHorizontal();
                
                GUILayout.Space(10);
                
                EditorGUI.BeginChangeCheck();
                _useAddressables = EditorGUILayout.Toggle("Use addressables", _useAddressables);
                if (EditorGUI.EndChangeCheck()
                    && _useAddressables
                    && !_enableAddressables)
                {
                    _useAddressables = false;
                    var isOpen = EditorUtility.DisplayDialog(
                        "Addressables Not Installed",
                        "Please install Addressables from the Package Manager to use this feature.",
                        "Open Package Manager",
                        "Close"
                    );
                    if (isOpen)
                    {
                        PackageInstaller.OpenPackageManager();
                    }
                }
                EditorGUI.EndDisabledGroup();
                
                GUILayout.Space(5);
                _useLocalization = EditorGUILayout.Toggle("Use localization", _useLocalization);
                GUILayout.Space(5);
                _isGenerateOutput = EditorGUILayout.Toggle("Generate output", _isGenerateOutput);
            }
            GUILayout.EndVertical();
            
            EditorGUI.BeginDisabledGroup(
                string.IsNullOrEmpty(_className)
                || string.IsNullOrEmpty(_buildPath)
            );
            {
                var style = new GUIStyle(GUI.skin.button)
                {
                    padding = new RectOffset(0,0,10,10),
                };
                
                GUILayout.BeginVertical( GUI.skin.box );

                foreach (var generator in ScriptGenerator.Generators)
                {
                    var texture = generator.GetLogo();
                    var isClicked = false;
                    var btnText = $" Generation - {generator.GetName()}";
                    if (texture != default)
                    {
                        var buttonContent = new GUIContent(btnText, texture);
                        isClicked = GUILayout.Button(buttonContent, style, GUILayout.Height(45));
                    }
                    else
                    {
                        isClicked = GUILayout.Button(btnText, style);
                    }

                    if (isClicked)
                    {
                        generator.Generate(_className, _buildPath, _isGenerateOutput, _useAddressables, _useLocalization);
                    }
                }

                GUILayout.EndVertical();
            }
            EditorGUI.EndDisabledGroup();
        }
    }
}