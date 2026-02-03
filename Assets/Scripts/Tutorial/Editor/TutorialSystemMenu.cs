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

        [MenuItem("Window/Tutorial System/Quick Setup", false, 101)]
        public static void QuickSetup()
        {
            var existingManager = Object.FindFirstObjectByType<TutorialManager>();
            if (existingManager != null)
            {
                EditorUtility.DisplayDialog("Info", "TutorialManager already exists in the scene", "OK");
                Selection.activeObject = existingManager.gameObject;
                return;
            }

            var canvas = Object.FindFirstObjectByType<Canvas>();
            if (canvas == null)
            {
                var canvasGo = new GameObject("Canvas");
                canvas = canvasGo.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasGo.AddComponent<UnityEngine.UI.CanvasScaler>();
                canvasGo.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            }

            var uiContainer = new GameObject("TutorialUIContainer");
            uiContainer.transform.SetParent(canvas.transform, false);
            var containerRect = uiContainer.AddComponent<RectTransform>();
            containerRect.anchorMin = Vector2.zero;
            containerRect.anchorMax = Vector2.one;
            containerRect.offsetMin = Vector2.zero;
            containerRect.offsetMax = Vector2.zero;

            var managerGo = new GameObject("TutorialManager");
            var manager = managerGo.AddComponent<TutorialManager>();

            var serializedManager = new SerializedObject(manager);
            serializedManager.FindProperty("uiContainer").objectReferenceValue = containerRect;
            serializedManager.FindProperty("targetCanvas").objectReferenceValue = canvas;
            serializedManager.ApplyModifiedProperties();

            Undo.RegisterCreatedObjectUndo(managerGo, "Quick Setup Tutorial System");
            
            Selection.activeObject = managerGo;
            EditorUtility.DisplayDialog("Complete", 
                "Tutorial System Quick Setup Complete!\n\n" +
                "Please create Arrow and Popup prefabs and assign them to TutorialManager.", 
                "OK");
        }

        [MenuItem("Window/Tutorial System/Documentation", false, 200)]
        public static void OpenDocumentation()
        {
            EditorUtility.DisplayDialog("Tutorial System Documentation",
                "1. Use 'Quick Setup' to create the basic structure\n\n" +
                "2. Create a 'TutorialRunner' component in the scene\n\n" +
                "3. Configure phases and steps in the TutorialRunner Inspector\n\n" +
                "4. Add triggers and modules for each step\n\n" +
                "5. You can directly reference scene UI objects (Button, RectTransform, etc.)\n\n" +
                "6. Use the Debug Window (Window > Tutorial System > Debug Window) for debugging",
                "OK");
        }
    }
}
