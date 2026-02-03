#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

namespace TutorialSystem.Setup
{
    /// <summary>
    /// 创建演示用的测试对象
    /// </summary>
    public static class TutorialDemoSetup
    {
        [MenuItem("Tutorial System/Setup/创建演示按钮", priority = 10)]
        public static void CreateDemoButton()
        {
            // 确保场景已初始化
            var canvas = Object.FindFirstObjectByType<Canvas>();
            if (canvas == null)
            {
                if (EditorUtility.DisplayDialog("缺少 Canvas",
                    "场景中没有 Canvas，是否先初始化场景？",
                    "初始化场景", "取消"))
                {
                    TutorialSceneSetup.SetupScene();
                    canvas = Object.FindFirstObjectByType<Canvas>();
                }
                else
                {
                    return;
                }
            }

            // 创建按钮
            var buttonGo = new GameObject("DemoButton");
            buttonGo.transform.SetParent(canvas.transform, false);

            var rect = buttonGo.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(200, 60);
            rect.anchoredPosition = new Vector2(0, -100);

            var image = buttonGo.AddComponent<Image>();
            image.color = new Color(0.2f, 0.6f, 1f);

            var button = buttonGo.AddComponent<Button>();
            button.targetGraphic = image;

            // 按钮文字
            var textGo = new GameObject("Text");
            textGo.transform.SetParent(buttonGo.transform, false);

            var textRect = textGo.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            var text = textGo.AddComponent<TextMeshProUGUI>();
            text.text = "点击这里";
            text.fontSize = 24;
            text.alignment = TextAlignmentOptions.Center;
            text.color = Color.white;

            Undo.RegisterCreatedObjectUndo(buttonGo, "Create Demo Button");
            Debug.Log("[TutorialDemoSetup] 已创建演示按钮");
        }

        [MenuItem("Tutorial System/Setup/创建完整演示", priority = 11)]
        public static void CreateFullDemo()
        {
            // 确保场景已初始化
            var manager = Object.FindFirstObjectByType<TutorialManager>();
            if (manager == null)
            {
                if (EditorUtility.DisplayDialog("缺少 TutorialManager",
                    "场景中没有 TutorialManager，是否先初始化场景？",
                    "初始化场景", "取消"))
                {
                    TutorialSceneSetup.SetupScene();
                }
                else
                {
                    return;
                }
            }

            // 创建演示按钮
            CreateDemoButton();

            var canvas = Object.FindFirstObjectByType<Canvas>();
            
            // 创建 TutorialRunner
            var runnerGo = new GameObject("TutorialRunner");
            var runner = runnerGo.AddComponent<TutorialRunner>();

            // 配置一个简单的教程
            var so = new SerializedObject(runner);
            var configProp = so.FindProperty("config");
            
            // 设置教程名称
            configProp.FindPropertyRelative("tutorialName").stringValue = "演示教程";
            configProp.FindPropertyRelative("tutorialId").stringValue = "demo_tutorial";
            configProp.FindPropertyRelative("canSkip").boolValue = true;
            
            // 设置自动启动
            so.FindProperty("autoStartOnEnable").boolValue = true;
            
            so.ApplyModifiedPropertiesWithoutUndo();

            Undo.RegisterCreatedObjectUndo(runnerGo, "Create TutorialRunner");

            Debug.Log("[TutorialDemoSetup] 演示设置完成!");
            EditorUtility.DisplayDialog("演示创建完成",
                "演示设置已创建。现在你需要手动配置 TutorialRunner 中的教程步骤：\n\n" +
                "1. 选择场景中的 TutorialRunner 对象\n" +
                "2. 在 Inspector 中添加 Phase\n" +
                "3. 在 Phase 中添加 Step\n" +
                "4. 配置 Trigger 和 Module\n" +
                "5. 点击 Play 测试", "确定");
        }

        [MenuItem("Tutorial System/Setup/创建代码演示对象", priority = 12)]
        public static void CreateCodeDemo()
        {
            // 确保场景已初始化
            var manager = Object.FindFirstObjectByType<TutorialManager>();
            if (manager == null)
            {
                if (EditorUtility.DisplayDialog("缺少 TutorialManager",
                    "场景中没有 TutorialManager，是否先初始化场景？",
                    "初始化场景", "取消"))
                {
                    TutorialSceneSetup.SetupScene();
                }
                else
                {
                    return;
                }
            }

            // 创建演示按钮
            CreateDemoButton();

            var canvas = Object.FindFirstObjectByType<Canvas>();
            var button = canvas.GetComponentInChildren<Button>();

            // 创建代码演示对象
            var demoGo = new GameObject("CodeTutorialDemo");
            var demo = demoGo.AddComponent<CodeTutorialDemo>();

            // 设置按钮引用
            var so = new SerializedObject(demo);
            so.FindProperty("targetButton").objectReferenceValue = button?.GetComponent<RectTransform>();
            so.ApplyModifiedPropertiesWithoutUndo();

            Undo.RegisterCreatedObjectUndo(demoGo, "Create Code Demo");

            Debug.Log("[TutorialDemoSetup] 代码演示对象创建完成!");
            EditorUtility.DisplayDialog("代码演示创建完成",
                "代码演示对象已创建。点击 Play 将自动启动一个通过代码构建的教程。", "确定");
        }
    }
}
#endif
