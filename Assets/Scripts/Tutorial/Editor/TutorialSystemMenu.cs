using UnityEngine;
using UnityEditor;

namespace TutorialSystem.Editor
{
    /// <summary>
    /// Tutorial System Menu
    /// </summary>
    public static class TutorialSystemMenu
    {
        [MenuItem("GameObject/Tutorial System/Tutorial Manager", false, 10)]
        public static void CreateTutorialManager(MenuCommand menuCommand)
        {
            var go = new GameObject("TutorialManager");
            go.AddComponent<TutorialManager>();

            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(go, "Create Tutorial Manager");
            Selection.activeObject = go;
        }

        [MenuItem("GameObject/Tutorial System/Tutorial Runner", false, 11)]
        public static void CreateTutorialRunner(MenuCommand menuCommand)
        {
            var go = new GameObject("TutorialRunner");
            go.AddComponent<TutorialRunner>();

            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(go, "Create Tutorial Runner");
            Selection.activeObject = go;
        }

        [MenuItem("Window/Tutorial System/Debug Window", false, 100)]
        public static void OpenDebugWindow()
        {
            TutorialDebugWindow.ShowWindow();
        }
    }
}
