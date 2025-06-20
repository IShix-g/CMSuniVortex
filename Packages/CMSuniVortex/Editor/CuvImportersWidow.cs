
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CMSuniVortex.Editor
{
    public sealed class CuvImportersWidow: EditorWindow
    {
        [MenuItem("Window/CMSuniVortex/open CuvImporter list")]
        public static void OpenCustomMenu()
        {
            var window = GetWindow<CuvImportersWidow>();
            window.titleContent = new GUIContent("CuvImporter list");
            window.Show();
        }
        
        Texture2D _logo;
        GUIContent _openButtonIcon;
        Vector2 _scrollPosition;
        ICuvImporterStatus[] _importers;
        
        void OnEnable()
        {
            _logo = CuvImporterEditor.GetLogo();
            _openButtonIcon = EditorGUIUtility.IconContent("Folder Icon");
            _importers = AssetDatabase.FindAssets("t:ScriptableObject")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<ScriptableObject>)
                .Select(obj => obj as ICuvImporterStatus)
                .Where(obj => obj != null)
                .ToArray();
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
                GUILayout.EndVertical();
            }
            
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);
            
            foreach (var importer in _importers)
            {
                GUILayout.BeginHorizontal(GUI.skin.box);
                {
                    var obj = (ScriptableObject) importer;
                    if (GUILayout.Button(_openButtonIcon, GUILayout.Width(30), GUILayout.Height(EditorGUIUtility.singleLineHeight)))
                    {
                        Selection.activeObject = obj;
                        EditorGUIUtility.PingObject(obj);
                    }
                    GUILayout.Label($"{importer.GetName()} ({importer.GetClintName()})");
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();
        }
    }
}