
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CMSuniVortex.Editor
{
    public sealed class CuvImportersWidow: EditorWindow
    {
        const float _dragHandleWidth = 80f;
        const float _dragHandleOffset = 3f;
        const float _minColumnWidth = 90f;
        const float _openButtonWidth = 30f;
        const float _clientIconWidth = 25f;
        
        [MenuItem("Window/CMSuniVortex/open CuvImporter list")]
        public static void OpenCustomMenu()
        {
            var window = GetWindow<CuvImportersWidow>();
            window.titleContent = new GUIContent("CuvImporter list");
            window.Show();
        }
        
        Texture2D _logo;
        GUIContent _openButtonIcon;
        GUIContent _linkIcon;
        IconState[] _clientIcons;
        ICuvImporterStatus[] _importers;
        Vector2 _scrollPosition;
        float _columnNameWidth = 90f;
        float _columnClientWidth = 200f;
        float _columnOutputWidth = 200f;
        bool _isDragging;
        int _draggingColumnIndex = -1;
        float _dragStartX;
        float _initialColumnWidth;

        class IconState
        {
            public readonly string Name;
            public readonly Texture2D Icon;

            public IconState(string name, Texture2D icon)
            {
                Name = name;
                Icon = icon;
            }
        }
        
        void OnEnable()
        {
            _logo = CuvImporterEditor.GetLogo();
            _openButtonIcon = EditorGUIUtility.IconContent("Folder Icon");
            _linkIcon = EditorGUIUtility.IconContent("d_Linked");
            _clientIcons = ScriptGenerator.Generators
                .Where(g => !string.IsNullOrEmpty(g.GetLogoName()))
                .Select(g => new IconState(g.GetName(), g.GetLogo()))
                .ToArray();
            _importers = AssetDatabase.FindAssets("t:ScriptableObject")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<ScriptableObject>)
                .Select(obj => obj as ICuvImporterStatus)
                .Where(obj => obj != null)
                .ToArray();
        }

        void OnGUI()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                var style = new GUIStyle(GUI.skin.label)
                {
                    padding = new RectOffset(5, 5, 5, 5),
                    alignment = TextAnchor.MiddleCenter,
                };
                GUILayout.Label(_logo, style, GUILayout.MinWidth(430), GUILayout.Height(75));
            }
            if (_importers != null
                && _importers.Length > 0)
            {
                EditorGUILayout.HelpBox("If the name is hard to read, you can drag the header text.", MessageType.Info);
            }
            EditorGUILayout.EndVertical();

            if (_importers == null
                || _importers.Length == 0)
            {
                EditorGUILayout.HelpBox("No CuvImporter found.", MessageType.Info);
                return;
            }
            
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
            GUILayout.Label(_linkIcon, GUILayout.Width(_openButtonWidth));
            GUILayout.Label(string.Empty, GUILayout.Width(_clientIconWidth));
            DrawResizableColumn("| Name", ref _columnNameWidth, 0);
            DrawResizableColumn("| Clint name", ref _columnClientWidth, 1);
            DrawResizableColumn("| Output name", ref _columnOutputWidth, 2);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        void DrawResizableColumn(string title, ref float columnWidth, int columnIndex)
        {
            GUILayout.Label(title, GUILayout.Width(columnWidth));

            var lastRect = GUILayoutUtility.GetLastRect();
            var dragRect = new Rect(lastRect.xMax - _dragHandleOffset, lastRect.y, _dragHandleWidth + _dragHandleOffset, lastRect.height);

            EditorGUIUtility.AddCursorRect(dragRect, MouseCursor.ResizeHorizontal);

            if (_isDragging
                && _draggingColumnIndex == columnIndex)
            {
                var mouseDelta = Event.current.mousePosition.x - _dragStartX;
                columnWidth = Mathf.Max(_minColumnWidth, _initialColumnWidth + mouseDelta);
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

            var clientIcon = GetClientIcon(status.GetClientName());
            GUILayout.Label(clientIcon, GUILayout.Width(_clientIconWidth), GUILayout.Height(EditorGUIUtility.singleLineHeight));
            GUILayout.Label(status.GetName(), GUILayout.Width(_columnNameWidth));
            GUILayout.Label(status.GetClintClassName(), GUILayout.Width(_columnClientWidth));
            GUILayout.Label(status.GetOutputClassName(), GUILayout.Width(_columnOutputWidth));
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }
        
        Texture2D GetClientIcon(string clientName)
            => _clientIcons.FirstOrDefault(g => g.Name == clientName)?.Icon;
    }
}