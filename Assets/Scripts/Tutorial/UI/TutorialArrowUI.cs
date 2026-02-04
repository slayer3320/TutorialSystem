using System;
using UnityEngine;
using UnityEngine.UI;

namespace TutorialSystem
{
    /// <summary>
    /// 箭头 UI 组件
    /// </summary>
    public class TutorialArrowUI : MonoBehaviour
    {
        [SerializeField] private Image arrowImage;
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private CanvasGroup canvasGroup;

        private ArrowModule module;
        private ArrowDirection direction;

        // 暴露RectTransform供Effect使用
        public RectTransform RectTransform => rectTransform;

        private void Awake()
        {
            if (rectTransform == null)
                rectTransform = GetComponent<RectTransform>();
            if (canvasGroup == null)
                canvasGroup = GetComponent<CanvasGroup>();
            if (arrowImage == null)
                arrowImage = GetComponent<Image>();
        }

        public void Setup(ArrowModule module, ArrowDirection direction, Color color, float scale)
        {
            this.module = module;
            this.direction = direction;

            if (arrowImage != null)
            {
                arrowImage.color = color;
            }

            rectTransform.localScale = Vector3.one * scale;
            ApplyRotation();
            UpdatePosition();
        }

        public void Show()
        {
            gameObject.SetActive(true);
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
            }
        }

        public void Hide()
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
            }
            gameObject.SetActive(false);
        }

        public void UpdatePosition()
        {
            if (module != null)
            {
                // 直接使用 Module 计算的位置
                rectTransform.anchoredPosition = module.GetTargetPosition(rectTransform);
            }
        }

        private void ApplyRotation()
        {
            float angle = direction switch
            {
                ArrowDirection.Up => 0f,
                ArrowDirection.Down => 180f,
                ArrowDirection.Left => 90f,
                ArrowDirection.Right => -90f,
                ArrowDirection.TopLeft => 45f,
                ArrowDirection.TopRight => -45f,
                ArrowDirection.BottomLeft => 135f,
                ArrowDirection.BottomRight => -135f,
                _ => 0f
            };

            rectTransform.localRotation = Quaternion.Euler(0, 0, angle);
        }
    }
}
