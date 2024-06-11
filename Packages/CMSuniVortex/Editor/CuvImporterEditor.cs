
using System;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace CMSuniVortex.Editor
{
    [CustomEditor(typeof(CuvImporter), true), CanEditMultipleObjects]
    public sealed class CuvImporterEditor : UnityEditor.Editor
    {
        const string _version = "v0.5.1";
        
        SerializedProperty _scriptProp;
        SerializedProperty _buildPathProp;
        SerializedProperty _languagesProp;
        SerializedProperty _clientProp;
        Type[] _types;
        string[] _options;
        int _selectedIndex;
        Texture2D _logo;
        Texture2D _importIcon;
        CuvImporter _myTarget;

        void OnEnable()
        {
            _scriptProp = serializedObject.FindProperty("m_Script");
            _buildPathProp = serializedObject.FindProperty("_buildPath");
            _languagesProp = serializedObject.FindProperty("_languages");
            _clientProp = serializedObject.FindProperty("_client");
            
            _types = TypeCache.GetTypesDerivedFrom<ICuvClient>()
                .Where(p => typeof(ICuvClient).IsAssignableFrom(p)
                            && !p.IsInterface
                            && !p.IsAbstract
                            && !p.GetCustomAttributes(typeof(IgnoreImporterAttribute), false).Any())
                .ToArray();

            _options = new string[_types.Length + 1];
            _options[0] = "Select..";
            for (var i = 0; i < _types.Length; i++)
            { 
                _options[i + 1] = _types[i].FullName;
            }
            
            _myTarget = target as CuvImporter;
            _logo = GetLogo();
            _importIcon = GetImportIcon();
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
            {
                var style = new GUIStyle(GUI.skin.label)
                {
                    alignment = TextAnchor.MiddleCenter,
                };
                EditorGUILayout.LabelField(_version, style, GUILayout.Width(80));
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical(GUI.skin.box);
            {
                var style = new GUIStyle(GUI.skin.label)
                {
                    padding = new RectOffset(5, 5, 5, 5),
                    alignment = TextAnchor.MiddleCenter,
                };
                GUILayout.Label(_logo, style, GUILayout.MinWidth(50), GUILayout.MaxHeight(80));
            }
            GUILayout.EndVertical();
            
            GUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(_buildPathProp);
            if (GUILayout.Button("Select", GUILayout.Width(60)))
            {
                if (!AssetDatabase.IsValidFolder(_buildPathProp.stringValue))
                {
                    _buildPathProp.stringValue = AssetDatabase.GetAssetPath(_myTarget);
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

            GUILayout.Space(5);
            
            {
                var fullTypeName = _clientProp.managedReferenceFullTypename;
                _selectedIndex = string.IsNullOrEmpty(fullTypeName) || fullTypeName == "  "
                    ? 0
                    : Array.FindIndex(_types, t => t.FullName == fullTypeName.Substring(fullTypeName.IndexOf(' ') + 1)) + 1;
                _selectedIndex = EditorGUILayout.Popup(_selectedIndex == 0 ? _clientProp.displayName : string.Empty, _selectedIndex, _options);

                if (_selectedIndex == 0)
                {
                    _clientProp.managedReferenceValue = default;
                }
                else
                {
                    GUILayout.Space(5);
                    
                    var selectedType = _types[_selectedIndex - 1];
                    if (fullTypeName != selectedType.Assembly.GetName().Name + " " + selectedType.FullName)
                    {
                        _clientProp.managedReferenceValue = Activator.CreateInstance(selectedType);
                    }
                    if (_clientProp.managedReferenceValue != default)
                    {
                        _clientProp.isExpanded = true;
                        EditorGUILayout.PropertyField(_clientProp, true);
                    }
                }
            }
            
            GUILayout.Space(10);

            EditorGUI.BeginDisabledGroup(_myTarget.IsLoading);

            var buttonContent = new GUIContent(_myTarget.IsLoading ? "  Now importing" : " Import", _importIcon);
            if (GUILayout.Button(buttonContent, GUILayout.Height(38)))
            {
                if (_myTarget.CanImport())
                {
                    _myTarget.StartImport();
                }
            }

            EditorGUI.EndDisabledGroup();
            
            serializedObject.ApplyModifiedProperties();
        }

        [CanBeNull]
        internal static Texture2D GetLogo() => GetTexture("CuvDataImporterLogo");
        
        [CanBeNull]
        internal static Texture2D GetImportIcon() => GetTexture("CuvImportIcon");
        
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
    }
}