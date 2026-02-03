#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

namespace TutorialSystem.Demo
{
    /// <summary>
    /// 创建教程系统所需的 UI 预制体
    /// </summary>
    public static class TutorialPrefabCreator
    {
        private const string PrefabFolder = "Assets/Prefabs/Tutorial";
        
        [MenuItem("Tutorial System/Demo/创建所有预制体", priority = 100)]
        public static void CreateAllPrefabs()
        {
            EnsureFolderExists();
            CreateArrowPrefab();
            CreatePopupPrefab();
            Debug.Log("[TutorialPrefabCreator] 所有预制体创建完成!");
            AssetDatabase.Refresh();
        }
        
        [MenuItem("Tutorial System/Demo/创建箭头预制体")]
        public static void CreateArrowPrefab()
        {
            EnsureFolderExists();
            
            var arrowGo = new GameObject("TutorialArrow");
            
            // RectTransform
            var rect = arrowGo.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(64, 64);
            
            // CanvasGroup
            arrowGo.AddComponent<CanvasGroup>();
            
            // Image (箭头图片)
            var image = arrowGo.AddComponent<Image>();
            image.color = Color.yellow;
            image.raycastTarget = false;
            
            // TutorialArrowUI 组件
            arrowGo.AddComponent<TutorialArrowUI>();
            
            // 保存预制体
            string path = $"{PrefabFolder}/TutorialArrow.prefab";
            PrefabUtility.SaveAsPrefabAsset(arrowGo, path);
            Object.DestroyImmediate(arrowGo);
            
            Debug.Log($"[TutorialPrefabCreator] 箭头预制体已创建: {path}");
        }
        
        [MenuItem("Tutorial System/Demo/创建弹窗预制体")]
        public static void CreatePopupPrefab()
        {
            EnsureFolderExists();
            
            // 主容器
            var popupGo = new GameObject("TutorialPopup");
            var popupRect = popupGo.AddComponent<RectTransform>();
            popupRect.sizeDelta = new Vector2(400, 200);
            popupGo.AddComponent<CanvasGroup>();
            
            // 背景
            var bgImage = popupGo.AddComponent<Image>();
            bgImage.color = new Color(0.1f, 0.1f, 0.1f, 0.95f);
            
            // 垂直布局
            var layout = popupGo.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(20, 20, 20, 20);
            layout.spacing = 10;
            layout.childAlignment = TextAnchor.UpperCenter;
            layout.childControlWidth = true;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
            
            // ContentSizeFitter
            var fitter = popupGo.AddComponent<ContentSizeFitter>();
            fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            
            // 标题
            var titleGo = new GameObject("Title");
            titleGo.transform.SetParent(popupGo.transform, false);
            var titleRect = titleGo.AddComponent<RectTransform>();
            titleRect.sizeDelta = new Vector2(0, 40);
            var titleText = titleGo.AddComponent<TextMeshProUGUI>();
            titleText.text = "标题";
            titleText.fontSize = 28;
            titleText.fontStyle = FontStyles.Bold;
            titleText.alignment = TextAlignmentOptions.Center;
            titleText.color = Color.white;
            
            // 内容
            var contentGo = new GameObject("Content");
            contentGo.transform.SetParent(popupGo.transform, false);
            var contentRect = contentGo.AddComponent<RectTransform>();
            contentRect.sizeDelta = new Vector2(0, 80);
            var contentText = contentGo.AddComponent<TextMeshProUGUI>();
            contentText.text = "这里是教程内容...";
            contentText.fontSize = 20;
            contentText.alignment = TextAlignmentOptions.Center;
            contentText.color = new Color(0.9f, 0.9f, 0.9f);
            
            // 按钮容器
            var btnContainerGo = new GameObject("ButtonContainer");
            btnContainerGo.transform.SetParent(popupGo.transform, false);
            var btnContainerRect = btnContainerGo.AddComponent<RectTransform>();
            btnContainerRect.sizeDelta = new Vector2(0, 50);
            
            // 按钮
            var buttonGo = new GameObject("ConfirmButton");
            buttonGo.transform.SetParent(btnContainerGo.transform, false);
            var btnRect = buttonGo.AddComponent<RectTransform>();
            btnRect.sizeDelta = new Vector2(120, 40);
            btnRect.anchoredPosition = Vector2.zero;
            
            var btnImage = buttonGo.AddComponent<Image>();
            btnImage.color = new Color(0.2f, 0.6f, 1f);
            
            var button = buttonGo.AddComponent<Button>();
            button.targetGraphic = btnImage;
            
            // 按钮文字
            var btnTextGo = new GameObject("Text");
            btnTextGo.transform.SetParent(buttonGo.transform, false);
            var btnTextRect = btnTextGo.AddComponent<RectTransform>();
            btnTextRect.anchorMin = Vector2.zero;
            btnTextRect.anchorMax = Vector2.one;
            btnTextRect.sizeDelta = Vector2.zero;
            btnTextRect.offsetMin = Vector2.zero;
            btnTextRect.offsetMax = Vector2.zero;
            
            var btnText = btnTextGo.AddComponent<TextMeshProUGUI>();
            btnText.text = "继续";
            btnText.fontSize = 18;
            btnText.alignment = TextAlignmentOptions.Center;
            btnText.color = Color.white;
            
            // TutorialPopupUI 组件
            var popupUI = popupGo.AddComponent<TutorialPopupUI>();
            
            // 通过 SerializedObject 设置私有字段
            var so = new SerializedObject(popupUI);
            so.FindProperty("titleText").objectReferenceValue = titleText;
            so.FindProperty("contentText").objectReferenceValue = contentText;
            so.FindProperty("confirmButton").objectReferenceValue = button;
            so.FindProperty("buttonText").objectReferenceValue = btnText;
            so.FindProperty("buttonContainer").objectReferenceValue = btnContainerRect;
            so.ApplyModifiedPropertiesWithoutUndo();
            
            // 保存预制体
            string path = $"{PrefabFolder}/TutorialPopup.prefab";
            PrefabUtility.SaveAsPrefabAsset(popupGo, path);
            Object.DestroyImmediate(popupGo);
            
            Debug.Log($"[TutorialPrefabCreator] 弹窗预制体已创建: {path}");
        }
        
        private static void EnsureFolderExists()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
                AssetDatabase.CreateFolder("Assets", "Prefabs");
            if (!AssetDatabase.IsValidFolder(PrefabFolder))
                AssetDatabase.CreateFolder("Assets/Prefabs", "Tutorial");
        }
    }
}
#endif
