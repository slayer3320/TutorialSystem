using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
        private RectTransform targetUI;
        private Transform worldTarget;
        private PositionMode positionMode;
        private ArrowDirection direction;
        private Vector2 offset;
        private Vector2 fixedPosition;
        private bool enableFloatAnimation;
        private float floatAmplitude;
        private float floatSpeed;
        private float floatTime;
        private Vector2 basePosition;
        private Camera mainCamera;

        private void Awake()
        {
            if (rectTransform == null)
                rectTransform = GetComponent<RectTransform>();
            if (canvasGroup == null)
                canvasGroup = GetComponent<CanvasGroup>();
            if (arrowImage == null)
                arrowImage = GetComponent<Image>();
                
            mainCamera = Camera.main;
        }

        public void Setup(ArrowModule module, RectTransform targetUI, Transform worldTarget,
            PositionMode positionMode, ArrowDirection direction, Vector2 offset, Color color, 
            float scale, bool enableFloatAnimation, float floatAmplitude, float floatSpeed, Vector2 fixedPosition)
        {
            this.module = module;
            this.targetUI = targetUI;
            this.worldTarget = worldTarget;
            this.positionMode = positionMode;
            this.direction = direction;
            this.offset = offset;
            this.fixedPosition = fixedPosition;
            this.enableFloatAnimation = enableFloatAnimation;
            this.floatAmplitude = floatAmplitude;
            this.floatSpeed = floatSpeed;
            this.floatTime = 0f;

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
            Vector2 targetPos = CalculateTargetPosition();
            basePosition = targetPos + offset;

            if (enableFloatAnimation)
            {
                floatTime += Time.deltaTime * floatSpeed;
                Vector2 floatOffset = GetFloatOffset();
                rectTransform.anchoredPosition = basePosition + floatOffset;
            }
            else
            {
                rectTransform.anchoredPosition = basePosition;
            }
        }

        private Vector2 CalculateTargetPosition()
        {
            switch (positionMode)
            {
                case PositionMode.FollowTarget:
                    if (targetUI != null)
                    {
                        return GetUIPosition(targetUI);
                    }
                    break;

                case PositionMode.WorldPosition:
                    if (worldTarget != null && mainCamera != null)
                    {
                        Vector3 screenPos = mainCamera.WorldToScreenPoint(worldTarget.position);
                        RectTransformUtility.ScreenPointToLocalPointInRectangle(
                            rectTransform.parent as RectTransform,
                            screenPos,
                            null,
                            out Vector2 localPos);
                        return localPos;
                    }
                    break;

                case PositionMode.Fixed:
                    return fixedPosition;
            }

            return Vector2.zero;
        }

        private Vector2 GetUIPosition(RectTransform target)
        {
            Vector3[] corners = new Vector3[4];
            target.GetWorldCorners(corners);
            Vector3 center = (corners[0] + corners[2]) / 2f;
            
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform.parent as RectTransform,
                RectTransformUtility.WorldToScreenPoint(null, center),
                null,
                out Vector2 localPos);
                
            return localPos;
        }

        private Vector2 GetFloatOffset()
        {
            float floatValue = Mathf.Sin(floatTime) * floatAmplitude;
            
            switch (direction)
            {
                case ArrowDirection.Up:
                case ArrowDirection.Down:
                    return new Vector2(0, floatValue);
                case ArrowDirection.Left:
                case ArrowDirection.Right:
                    return new Vector2(floatValue, 0);
                default:
                    return new Vector2(floatValue * 0.707f, floatValue * 0.707f);
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
