using UnityEngine;
using UnityEditor;

namespace TutorialSystem.Editor
{
    /// <summary>
    /// 教程系统菜单
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
                EditorUtility.DisplayDialog("提示", "场景中已存在 TutorialManager", "确定");
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
            EditorUtility.DisplayDialog("完成", 
                "教程系统快速设置完成！\n\n" +
                "请创建箭头和弹窗预制体并分配给 TutorialManager。", 
                "确定");
        }

        [MenuItem("Window/Tutorial System/Documentation", false, 200)]
        public static void OpenDocumentation()
        {
            EditorUtility.DisplayDialog("教程系统使用说明",
                "1. 使用 Quick Setup 快速创建基础结构\n\n" +
                "2. 在场景中创建 TutorialRunner 组件\n\n" +
                "3. 在 TutorialRunner 的 Inspector 中配置教程阶段和步骤\n\n" +
                "4. 为每个步骤添加触发器和模块\n\n" +
                "5. 可以直接引用场景中的 UI 对象（Button、RectTransform 等）\n\n" +
                "6. 使用调试窗口 (Window > Tutorial System > Debug Window) 进行调试",
                "确定");
        }
    }
}
