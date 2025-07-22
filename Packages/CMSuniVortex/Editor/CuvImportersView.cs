
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CMSuniVortex.Editor
{
    public sealed class CuvImportersView : IDisposable
    {
        const float _dragHandleWidth = 80f;
        const float _dragHandleOffset = 3f;
        const float _minColumnWidth = 90f;
        const float _openButtonWidth = 30f;
        const float _clientIconWidth = 25f;
        const float _languageIconWidth = 25f;
        const float _widthOffset = 50f;
        
        GUIContent _openButtonIcon;
        GUIContent _linkIcon;
        Texture2D _languageIcon;
        IconState[] _clientIcons;
        Vector2 _scrollPosition;

        float _columnNameWidth = _minColumnWidth;
        float _columnClientWidth = 180f;

        bool _isDragging;
        int _draggingColumnIndex = -1;
        float _dragStartX;
        float _initialColumnWidth;
        float _outputWidth;

        ICuvImporterStatus[] _importers;
        Action _repaintAction;
        Func<float> _getWidthFunc;

        public sealed class IconState
        {
            public readonly string Name;
            public readonly Texture2D Icon;

            public IconState(string name, Texture2D icon)
            {
                Name = name;
                Icon = icon;
            }
        }
        
        public CuvImportersView(ICuvImporterStatus[] importers, IconState[] clientIcons, Action repaintAction, Func<float> getWidthFunc)
        {
            _importers = importers;
            _clientIcons = clientIcons;
            _repaintAction = repaintAction;
            _getWidthFunc = getWidthFunc;
            _openButtonIcon = EditorGUIUtility.IconContent("Folder Icon");
            _linkIcon = EditorGUIUtility.IconContent("d_Linked");
            _languageIcon = CuvImporterEditor.GetTexture("LanguageIcon");
        }

        public void Update()
        {
            if (_importers is {Length: > 0})
            {
                EditorGUILayout.HelpBox("If the name is hard to read, you can drag the header text.", MessageType.Info);
            }
            else
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
            GUILayout.Label(_linkIcon, GUILayout.Width(_openButtonWidth + _clientIconWidth + _languageIconWidth + 10));
            DrawResizableColumn("| Name", ref _columnNameWidth, 0);
            DrawResizableColumn("| Client name", ref _columnClientWidth, 1);
            _outputWidth = _getWidthFunc() - (_widthOffset + _openButtonWidth + _clientIconWidth + _languageIconWidth + _columnNameWidth + _columnClientWidth);
            DrawResizableColumn("| Output name", ref _outputWidth, 2);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        void DrawResizableColumn(string title, ref float columnWidth, int columnIndex)
        {
            GUILayout.Label(title, GUILayout.Width(columnWidth));

            var lastRect = GUILayoutUtility.GetLastRect();
            var dragRect = new Rect(
                    lastRect.xMax - _dragHandleOffset,
                    lastRect.y,
                    _dragHandleWidth + _dragHandleOffset,
                    lastRect.height
                );

            EditorGUIUtility.AddCursorRect(dragRect, MouseCursor.ResizeHorizontal);

            if (_isDragging
                && _draggingColumnIndex == columnIndex)
            {
                var mouseDelta = Event.current.mousePosition.x - _dragStartX;
                columnWidth = Mathf.Max(_minColumnWidth, _initialColumnWidth + mouseDelta);
                _repaintAction?.Invoke();
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
                Selection.activeObject = (ScriptableObject) status;
                EditorGUIUtility.PingObject((Object) status);
            }

            var clientIcon = GetClientIcon(status.GetClientName());
            GUILayout.Label(clientIcon, GUILayout.Width(_clientIconWidth), GUILayout.Height(EditorGUIUtility.singleLineHeight));
            GUILayout.Label(status.IsLocalization() ? _languageIcon : default, GUILayout.Width(_languageIconWidth), GUILayout.Height(EditorGUIUtility.singleLineHeight));
            GUILayout.Label(status.GetName(), GUILayout.Width(_columnNameWidth));
            GUILayout.Label(status.GetClintClassName(), GUILayout.Width(_columnClientWidth));
            _outputWidth = _getWidthFunc() - (_widthOffset + _openButtonWidth + _clientIconWidth + _languageIconWidth + _columnNameWidth + _columnClientWidth);
            GUILayout.Label(status.GetOutputClassName(), GUILayout.Width(_outputWidth));
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        Texture2D GetClientIcon(string clientName)
            => _clientIcons.FirstOrDefault(g => g.Name == clientName)?.Icon;
        
        public void Dispose()
        {
            _importers = default;
            _repaintAction = default;
            _getWidthFunc = default;
            _openButtonIcon = default;
            _linkIcon = default;
            _languageIcon = default;
            _clientIcons = default;
        }
    }
}