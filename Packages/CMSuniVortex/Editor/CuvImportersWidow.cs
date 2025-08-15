
using UnityEditor;
using UnityEngine;

namespace CMSuniVortex.Editor
{
    internal sealed class CuvImportersWidow : EditorWindow
    {
        [MenuItem("Window/CMSuniVortex/open CuvImporter list")]
        public static void ShowWindow()
        {
            var window = GetWindow<CuvImportersWidow>();
            window.titleContent = new GUIContent("CuvImporter list");
            window.Show();
        }
 
        static ICuvImporterStatus[] s_importers;
        static CuvImportersView.IconState[] s_iconStates;
        Texture2D _logo;
        CuvImportersView _importersView;
        
        void OnEnable()
        {
            _logo = CuvImporterEditor.GetLogo();
            s_importers ??= CuvImportersCache.FilterImporters<ICuvImporterStatus>();
            s_iconStates ??= CuvImportersCache.IconStates;
            _importersView = new CuvImportersView(s_importers, s_iconStates, Repaint, ClickAction, () => position.width);
        }

        void OnDisable()
        {
            _logo = default;
            _importersView.Dispose();
            s_importers = default;
            s_iconStates = default;
        }

        void ClickAction(ICuvImporterStatus status)
            => CuvImporterInspectorWindow.Show(status);
        
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
            EditorGUILayout.EndVertical();
            
            _importersView.Update();
        }
    }
}