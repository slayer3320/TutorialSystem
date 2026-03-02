using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TutorialSystem
{
    /// <summary>
    /// 弹窗 UI 组件
    /// </summary>
    public class TutorialPopupUI : MonoBehaviour
    {
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private CanvasGroup canvasGroup;

        // 暴露CanvasGroup供Effect使用
        public CanvasGroup CanvasGroup => canvasGroup;

        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI contentText;
        [SerializeField] private Button confirmButton;
        [SerializeField] private TextMeshProUGUI buttonText;
        [SerializeField] private RectTransform buttonContainer;

        public event Action OnButtonClick;

        private PopupPosition position;
        private Vector2 customPosition;
        private Vector2 offset;
        private Canvas parentCanvas;

        private void Awake()
        {
            if (rectTransform == null)
                rectTransform = GetComponent<RectTransform>();
            if (canvasGroup == null)
                canvasGroup = GetComponent<CanvasGroup>();

            if (confirmButton != null)
            {
                confirmButton.onClick.AddListener(HandleButtonClick);
            }
        }

        private void OnDestroy()
        {
            if (confirmButton != null)
            {
                confirmButton.onClick.RemoveListener(HandleButtonClick);
            }
        }

        public void Setup(string title, string content, bool showButton, string btnText,
            PopupPosition position, Vector2 customPosition, Vector2 offset, float width)
        {
            this.position = position;
            this.customPosition = customPosition;
            this.offset = offset;

            if (titleText != null)
            {
                titleText.text = title;
                titleText.gameObject.SetActive(!string.IsNullOrEmpty(title));
            }

            if (contentText != null)
            {
                contentText.text = content;
            }

            if (buttonContainer != null)
            {
                buttonContainer.gameObject.SetActive(showButton);
            }

            if (confirmButton != null)
            {
                confirmButton.gameObject.SetActive(showButton);
            }

            if (buttonText != null)
            {
                buttonText.text = btnText;
            }

            if (width > 0 && rectTransform != null)
            {
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            }

            ApplyPosition();
        }

        public void Show()
        {
            gameObject.SetActive(true);
            //GetComponent<VerticalLayoutGroup>().SetLayoutVertical();
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

        private void ApplyPosition()
        {
            if (rectTransform == null) return;

            parentCanvas = GetComponentInParent<Canvas>();
            if (parentCanvas == null) return;

            RectTransform canvasRect = parentCanvas.GetComponent<RectTransform>();
            Vector2 canvasSize = canvasRect.rect.size;

            Vector2 targetPos = position switch
            {
                PopupPosition.Center => Vector2.zero,
                PopupPosition.Top => new Vector2(0, canvasSize.y / 4),
                PopupPosition.Bottom => new Vector2(0, -canvasSize.y / 4),
                PopupPosition.Left => new Vector2(-canvasSize.x / 4, 0),
                PopupPosition.Right => new Vector2(canvasSize.x / 4, 0),
                PopupPosition.TopLeft => new Vector2(-canvasSize.x / 4, canvasSize.y / 4),
                PopupPosition.TopRight => new Vector2(canvasSize.x / 4, canvasSize.y / 4),
                PopupPosition.BottomLeft => new Vector2(-canvasSize.x / 4, -canvasSize.y / 4),
                PopupPosition.BottomRight => new Vector2(canvasSize.x / 4, -canvasSize.y / 4),
                PopupPosition.Custom => customPosition,
                _ => Vector2.zero
            };

            rectTransform.anchoredPosition = targetPos + offset;
        }

        private void HandleButtonClick()
        {
            OnButtonClick?.Invoke();
        }
    }
}
