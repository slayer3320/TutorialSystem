#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor;

namespace TutorialSystem.Setup
{
    /// <summary>
    /// 教程场景初始化工具
    /// </summary>
    public static class TutorialSceneSetup
    {
        private const string ArrowPrefabPath = "Assets/Prefabs/Tutorial/TutorialArrow.prefab";
        private const string PopupPrefabPath = "Assets/Prefabs/Tutorial/TutorialPopup.prefab";

        [MenuItem("Tutorial System/Setup/初始化场景", priority = 0)]
        public static void SetupScene()
        {
            // 检查预制体是否存在
            if (!CheckPrefabsExist())
            {
                if (EditorUtility.DisplayDialog("缺少预制体",
                    "未找到 UI 预制体，是否先创建预制体？",
                    "创建预制体", "取消"))
                {
                    Demo.TutorialPrefabCreator.CreateAllPrefabs();
                }
                else
                {
                    return;
                }
            }

            // 创建 EventSystem
            CreateEventSystem();

            // 创建 Canvas
            var canvas = CreateCanvas();

            // 创建 TutorialManager
            CreateTutorialManager(canvas.transform);

            Debug.Log("[TutorialSceneSetup] 场景初始化完成!");
            EditorUtility.DisplayDialog("初始化完成", "教程系统场景初始化完成！", "确定");
        }

        [MenuItem("Tutorial System/Setup/检查场景配置", priority = 1)]
        public static void ValidateScene()
        {
            bool hasIssues = false;
            
            // 检查 EventSystem
            if (Object.FindFirstObjectByType<EventSystem>() == null)
            {
                Debug.LogWarning("[TutorialSceneSetup] 场景缺少 EventSystem");
                hasIssues = true;
            }

            // 检查 Canvas
            if (Object.FindFirstObjectByType<Canvas>() == null)
            {
                Debug.LogWarning("[TutorialSceneSetup] 场景缺少 Canvas");
                hasIssues = true;
            }

            // 检查 TutorialManager
            if (Object.FindFirstObjectByType<TutorialManager>() == null)
            {
                Debug.LogWarning("[TutorialSceneSetup] 场景缺少 TutorialManager");
                hasIssues = true;
            }

            // 检查预制体
            if (!CheckPrefabsExist())
            {
                Debug.LogWarning("[TutorialSceneSetup] 缺少 UI 预制体，请执行 Tutorial System/Demo/创建所有预制体");
                hasIssues = true;
            }

            if (!hasIssues)
            {
                Debug.Log("[TutorialSceneSetup] 场景配置正确!");
                EditorUtility.DisplayDialog("检查通过", "场景配置正确，可以运行教程系统。", "确定");
            }
            else
            {
                EditorUtility.DisplayDialog("配置问题", "场景存在配置问题，请查看 Console 日志。", "确定");
            }
        }

        private static bool CheckPrefabsExist()
        {
            var arrow = AssetDatabase.LoadAssetAtPath<GameObject>(ArrowPrefabPath);
            var popup = AssetDatabase.LoadAssetAtPath<GameObject>(PopupPrefabPath);
            return arrow != null && popup != null;
        }

        private static void CreateEventSystem()
        {
            if (Object.FindFirstObjectByType<EventSystem>() != null)
            {
                Debug.Log("[TutorialSceneSetup] EventSystem 已存在");
                return;
            }

            var eventSystemGo = new GameObject("EventSystem");
            eventSystemGo.AddComponent<EventSystem>();
            eventSystemGo.AddComponent<StandaloneInputModule>();
            Undo.RegisterCreatedObjectUndo(eventSystemGo, "Create EventSystem");
            Debug.Log("[TutorialSceneSetup] 已创建 EventSystem");
        }

        private static Canvas CreateCanvas()
        {
            // 查找现有 Canvas
            var existingCanvas = Object.FindFirstObjectByType<Canvas>();
            if (existingCanvas != null)
            {
                Debug.Log("[TutorialSceneSetup] Canvas 已存在");
                
                // 确保有 UIContainer
                EnsureUIContainer(existingCanvas.transform);
                return existingCanvas;
            }

            // 创建新 Canvas
            var canvasGo = new GameObject("TutorialCanvas");
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100; // 确保在最上层
            
            canvasGo.AddComponent<CanvasScaler>();
            canvasGo.AddComponent<GraphicRaycaster>();

            Undo.RegisterCreatedObjectUndo(canvasGo, "Create Canvas");

            // 创建 UIContainer
            EnsureUIContainer(canvasGo.transform);

            Debug.Log("[TutorialSceneSetup] 已创建 Canvas");
            return canvas;
        }

        private static void EnsureUIContainer(Transform canvasTransform)
        {
            var existingContainer = canvasTransform.Find("UIContainer");
            if (existingContainer != null)
            {
                Debug.Log("[TutorialSceneSetup] UIContainer 已存在");
                return;
            }

            var containerGo = new GameObject("UIContainer");
            containerGo.transform.SetParent(canvasTransform, false);

            var rect = containerGo.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            Undo.RegisterCreatedObjectUndo(containerGo, "Create UIContainer");
            Debug.Log("[TutorialSceneSetup] 已创建 UIContainer");
        }

        private static void CreateTutorialManager(Transform canvasTransform)
        {
            // 查找现有 TutorialManager
            var existingManager = Object.FindFirstObjectByType<TutorialManager>();
            if (existingManager != null)
            {
                Debug.Log("[TutorialSceneSetup] TutorialManager 已存在");
                return;
            }

            // 创建 TutorialManager
            var managerGo = new GameObject("TutorialManager");
            var manager = managerGo.AddComponent<TutorialManager>();

            // 加载预制体
            var arrowPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(ArrowPrefabPath);
            var popupPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PopupPrefabPath);

            // 获取 UIContainer
            var uiContainer = canvasTransform.Find("UIContainer");
            var canvas = canvasTransform.GetComponent<Canvas>();

            // 通过 SerializedObject 设置私有字段
            var so = new SerializedObject(manager);
            so.FindProperty("arrowPrefab").objectReferenceValue = arrowPrefab;
            so.FindProperty("popupPrefab").objectReferenceValue = popupPrefab;
            so.FindProperty("uiContainer").objectReferenceValue = uiContainer;
            so.FindProperty("targetCanvas").objectReferenceValue = canvas;
            so.FindProperty("debugMode").boolValue = true;
            so.ApplyModifiedPropertiesWithoutUndo();

            Undo.RegisterCreatedObjectUndo(managerGo, "Create TutorialManager");
            Debug.Log("[TutorialSceneSetup] 已创建 TutorialManager");
        }
    }
}
#endif
