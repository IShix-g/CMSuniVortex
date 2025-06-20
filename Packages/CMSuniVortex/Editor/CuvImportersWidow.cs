
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CMSuniVortex.Editor
{
    public sealed class CuvImportersWidow: EditorWindow
    {
        const float _dragHandleWidth = 15f;
        const float _maxColumnWidth = 90f;
        const float _openButtonWidth = 30f;
        
        [MenuItem("Window/CMSuniVortex/open CuvImporter list")]
        public static void OpenCustomMenu()
        {
            var window = GetWindow<CuvImportersWidow>();
            window.titleContent = new GUIContent("CuvImporter list");
            window.Show();
        }
        
        GUIContent _openButtonIcon;
        ICuvImporterStatus[] _importers;
        Vector2 _scrollPosition;
        float _columnNameWidth = 90f;
        float _columnClientWidth = 200f;
        float _columnOutputWidth = 200f;
        bool _isDragging;
        int _draggingColumnIndex = -1;
        float _dragStartX;
        float _initialColumnWidth;
        
        void OnEnable()
        {
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
            DrawHeader();
            
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);
            foreach (var importer in _importers)
            {
                DrawRow(importer);
            }
            GUILayout.EndScrollView();
        }
        
        void DrawHeader()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.Label("", GUILayout.Width(_openButtonWidth));
            DrawResizableColumn("Name", ref _columnNameWidth, 0);
            DrawResizableColumn("Clint Name", ref _columnClientWidth, 1);
            DrawResizableColumn("Output Name", ref _columnOutputWidth, 2);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        void DrawResizableColumn(string title, ref float columnWidth, int columnIndex)
        {
            GUILayout.Label(title, GUILayout.Width(columnWidth));

            var lastRect = GUILayoutUtility.GetLastRect();
            var dragRect = new Rect(lastRect.xMax - _dragHandleWidth / 2, lastRect.y, _dragHandleWidth, lastRect.height);

            EditorGUIUtility.AddCursorRect(dragRect, MouseCursor.ResizeHorizontal);

            if (_isDragging
                && _draggingColumnIndex == columnIndex)
            {
                var mouseDelta = Event.current.mousePosition.x - _dragStartX;
                columnWidth = Mathf.Max(_maxColumnWidth, _initialColumnWidth + mouseDelta);
                Repaint();
            }

            if (Event.current.type == EventType.MouseDown
                && dragRect.Contains(Event.current.mousePosition))
            {
                _isDragging = true;
                _draggingColumnIndex = columnIndex;
                _dragStartX = Event.current.mousePosition.x;
                _initialColumnWidth = columnWidth;
                Event.current.Use();
            }
            else if (Event.current.type == EventType.MouseUp)
            {
                _isDragging = false;
                _draggingColumnIndex = -1;
            }
        }

        void DrawRow(ICuvImporterStatus status)
        {
            var style = new GUIStyle(EditorStyles.helpBox)
            {
                padding = new RectOffset(5, 5, 5, 5)
            };
            EditorGUILayout.BeginHorizontal(style);
            if (GUILayout.Button(
                _openButtonIcon,
                GUILayout.Width(_openButtonWidth),
                GUILayout.Height(EditorGUIUtility.singleLineHeight)
            ))
            {
                var obj = (ScriptableObject) status;
                Selection.activeObject = obj;
                EditorGUIUtility.PingObject(obj);
            }
            GUILayout.Label(status.GetName(), GUILayout.Width(_columnNameWidth));
            GUILayout.Label(status.GetClintName(), GUILayout.Width(_columnClientWidth));
            GUILayout.Label(status.GetOutputName(), GUILayout.Width(_columnOutputWidth));
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }
    }
}