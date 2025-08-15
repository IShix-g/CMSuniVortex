
using UnityEngine;
using UnityEditor;

namespace CMSuniVortex.Editor
{
    public sealed class CuvImporterInspectorWindow : EditorWindow
    {
        Vector2 _scrollPos;
        ScriptableObject _obj;
        UnityEditor.Editor _editor;
        GUIContent _icon;

        public static void Show(ICuvImporterStatus status)
        {
            var window = GetWindow<CuvImporterInspectorWindow>();
            var obj = (ScriptableObject) status;
            window._obj = obj;
            window._editor = UnityEditor.Editor.CreateEditor(obj);
            window.titleContent = new GUIContent(obj.name);
            window.minSize = new Vector2(450, 800);
            window.Show();
        }

        void OnEnable() => _icon = EditorGUIUtility.IconContent("ScriptableObject Icon");

        void OnDisable() => _icon = default;

        void OnGUI()
        {
            if (_obj == default)
            {
                Close();
                return;
            }
            
            GUILayout.BeginVertical();
            _icon.text = " Select " + _obj.name;
            if (GUILayout.Button(_icon, GUILayout.Height(30), GUILayout.ExpandWidth(true)))
            {
                Selection.activeObject = _obj;
                EditorGUIUtility.PingObject(_obj);
            }
            GUILayout.EndVertical();
            GUILayout.Space(10);
            
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
            _editor.OnInspectorGUI();
            EditorGUILayout.EndScrollView();
        }
    }
}