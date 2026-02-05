using UnityEngine;
using UnityEngine.UI;

namespace TutorialSystem
{
    /// <summary>
    /// Sprite UI 组件 - 用于显示框选高亮
    /// </summary>
    public class TutorialSpriteUI : MonoBehaviour
    {
        [SerializeField] private Image spriteImage;
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private CanvasGroup canvasGroup;

        private SpriteModule module;

        // 暴露RectTransform供Effect使用
        public RectTransform RectTransform => rectTransform;

        private void Awake()
        {
            if (rectTransform == null)
                rectTransform = GetComponent<RectTransform>();
            if (canvasGroup == null)
                canvasGroup = GetComponent<CanvasGroup>();
            if (spriteImage == null)
                spriteImage = GetComponent<Image>();
        }

        public void Setup(SpriteModule module, Sprite sprite, Color color, Vector2 size, bool preserveAspect)
        {
            this.module = module;

            if (spriteImage != null)
            {
                spriteImage.sprite = sprite;
                spriteImage.color = color;
                spriteImage.preserveAspect = preserveAspect;
            }

            // 设置尺寸
            if (size != Vector2.zero)
            {
                rectTransform.sizeDelta = size;
            }
            else if (sprite != null)
            {
                // 如果没有自定义尺寸，使用sprite的原始尺寸
                rectTransform.sizeDelta = new Vector2(sprite.rect.width, sprite.rect.height);
            }

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
    }
}
