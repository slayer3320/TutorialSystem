using System;
using UnityEngine;
using UnityEngine.UI;

namespace TutorialSystem
{
    /// <summary>
    /// 高亮 UI 组件
    /// </summary>
    public class TutorialHighlightUI : MonoBehaviour
    {
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Image maskImage;
        [SerializeField] private Image borderImage;
        [SerializeField] private RectTransform borderRect;

        private HighlightType highlightType;
        private RectTransform targetUI;
        private Vector2 padding;
        private bool enablePulseAnimation;
        private float pulseSpeed;
        private float pulseAmplitude;
        private float animationTime;
        private Canvas targetCanvas;
        private bool clickMaskToNext;

        public event Action OnMaskClicked;

        private void Awake()
        {
            if (rectTransform == null)
                rectTransform = GetComponent<RectTransform>();
            if (canvasGroup == null)
                canvasGroup = GetComponent<CanvasGroup>();
        }

        public void Setup(RectTransform target, HighlightType type, HighlightShape shape,
            Color maskColor, Color highlightColor, Vector2 padding, float cornerRadius,
            bool enablePulse, float pulseSpeed, float pulseAmplitude,
            bool blockRaycasts, bool clickToNext, Canvas canvas)
        {
            this.targetUI = target;
            this.highlightType = type;
            this.padding = padding;
            this.enablePulseAnimation = enablePulse;
            this.pulseSpeed = pulseSpeed;
            this.pulseAmplitude = pulseAmplitude;
            this.targetCanvas = canvas;
            this.clickMaskToNext = clickToNext;
            this.animationTime = 0f;

            SetupHighlightType(type, maskColor, highlightColor, blockRaycasts);
            UpdatePosition();
        }

        private void SetupHighlightType(HighlightType type, Color maskColor, Color highlightColor, bool blockRaycasts)
        {
            // 清理现有子对象
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            switch (type)
            {
                case HighlightType.CutoutMask:
                    CreateCutoutMask(maskColor, highlightColor, blockRaycasts);
                    break;
                case HighlightType.GlowBorder:
                    CreateGlowBorder(highlightColor);
                    break;
                case HighlightType.PulseScale:
                    CreatePulseHighlight(highlightColor);
                    break;
            }
        }

        private void CreateCutoutMask(Color maskColor, Color highlightColor, bool blockRaycasts)
        {
            maskImage = gameObject.GetComponent<Image>();
            if (maskImage == null)
                maskImage = gameObject.AddComponent<Image>();

            maskImage.color = maskColor;
            maskImage.raycastTarget = blockRaycasts;

            if (clickMaskToNext)
            {
                var button = gameObject.GetComponent<Button>();
                if (button == null)
                    button = gameObject.AddComponent<Button>();

                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => OnMaskClicked?.Invoke());
            }

            var cutoutObj = new GameObject("Cutout");
            cutoutObj.transform.SetParent(transform, false);

            borderRect = cutoutObj.AddComponent<RectTransform>();
            borderImage = cutoutObj.AddComponent<Image>();
            borderImage.color = new Color(1, 1, 1, 0);
            borderImage.raycastTarget = false;

            var outline = cutoutObj.AddComponent<Outline>();
            outline.effectColor = highlightColor;
            outline.effectDistance = new Vector2(3, 3);
        }

        private void CreateGlowBorder(Color highlightColor)
        {
            var borderObj = new GameObject("Border");
            borderObj.transform.SetParent(transform, false);

            borderRect = borderObj.AddComponent<RectTransform>();
            borderImage = borderObj.AddComponent<Image>();
            borderImage.color = highlightColor;
            borderImage.raycastTarget = false;

            var outline = borderObj.AddComponent<Outline>();
            outline.effectColor = highlightColor;
            outline.effectDistance = new Vector2(5, 5);
        }

        private void CreatePulseHighlight(Color highlightColor)
        {
            var pulseObj = new GameObject("Pulse");
            pulseObj.transform.SetParent(transform, false);

            borderRect = pulseObj.AddComponent<RectTransform>();
            borderImage = pulseObj.AddComponent<Image>();
            borderImage.color = new Color(highlightColor.r, highlightColor.g, highlightColor.b, 0.3f);
            borderImage.raycastTarget = false;
        }

        public void Show()
        {
            gameObject.SetActive(true);
            if (canvasGroup != null)
                canvasGroup.alpha = 1f;
        }

        public void Hide()
        {
            if (canvasGroup != null)
                canvasGroup.alpha = 0f;
            gameObject.SetActive(false);
        }

        public void UpdateHighlight()
        {
            UpdatePosition();

            if (enablePulseAnimation && borderImage != null)
            {
                animationTime += Time.deltaTime * pulseSpeed;
                float scale = 1f + Mathf.Sin(animationTime) * pulseAmplitude;
                borderImage.transform.localScale = Vector3.one * scale;

                float alpha = 0.3f + Mathf.Sin(animationTime) * 0.2f;
                var color = borderImage.color;
                color.a = alpha;
                borderImage.color = color;
            }
        }

        private void UpdatePosition()
        {
            if (targetUI == null || borderRect == null || targetCanvas == null) return;

            Vector3[] corners = new Vector3[4];
            targetUI.GetWorldCorners(corners);

            RectTransform canvasRect = targetCanvas.GetComponent<RectTransform>();
            Camera cam = targetCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : targetCanvas.worldCamera;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect,
                RectTransformUtility.WorldToScreenPoint(cam, corners[0]), cam, out Vector2 min);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect,
                RectTransformUtility.WorldToScreenPoint(cam, corners[2]), cam, out Vector2 max);

            borderRect.anchorMin = new Vector2(0.5f, 0.5f);
            borderRect.anchorMax = new Vector2(0.5f, 0.5f);
            borderRect.anchoredPosition = (min + max) / 2;
            borderRect.sizeDelta = new Vector2(max.x - min.x + padding.x * 2, max.y - min.y + padding.y * 2);
        }

        public void Reset()
        {
            targetUI = null;
            animationTime = 0f;

            if (maskImage != null)
            {
                var button = GetComponent<Button>();
                if (button != null)
                    button.onClick.RemoveAllListeners();
            }

            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
        }
    }
}
