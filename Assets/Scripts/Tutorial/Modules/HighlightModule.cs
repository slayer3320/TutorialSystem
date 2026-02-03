using System;
using UnityEngine;
using UnityEngine.UI;

namespace TutorialSystem
{
    /// <summary>
    /// 高亮类型
    /// </summary>
    public enum HighlightType
    {
        CutoutMask,
        GlowBorder,
        PulseScale
    }

    /// <summary>
    /// 高亮形状
    /// </summary>
    public enum HighlightShape
    {
        Rectangle,
        Circle,
        RoundedRectangle
    }

    /// <summary>
    /// 高亮模块
    /// </summary>
    [Serializable]
    public class HighlightModule : TutorialModuleBase
    {
        public override string ModuleName => "高亮";

        [Header("目标设置")]
        [SerializeField]
        [Tooltip("目标 UI 元素")]
        private RectTransform targetUI;

        [SerializeField]
        [Tooltip("目标路径（运行时查找）")]
        private string targetPath;

        [SerializeField]
        [Tooltip("边距")]
        private Vector2 padding = new Vector2(10f, 10f);

        [Header("高亮设置")]
        [SerializeField]
        private HighlightType highlightType = HighlightType.CutoutMask;

        [SerializeField]
        private HighlightShape shape = HighlightShape.Rectangle;

        [SerializeField]
        [Tooltip("遮罩颜色")]
        private Color maskColor = new Color(0, 0, 0, 0.7f);

        [SerializeField]
        [Tooltip("高亮颜色")]
        private Color highlightColor = Color.yellow;

        [SerializeField]
        [Tooltip("圆角半径")]
        private float cornerRadius = 10f;

        [Header("动画设置")]
        [SerializeField]
        private bool enablePulseAnimation = true;

        [SerializeField]
        private float pulseSpeed = 2f;

        [SerializeField]
        private float pulseAmplitude = 0.1f;

        [Header("交互设置")]
        [SerializeField]
        [Tooltip("阻挡非高亮区域点击")]
        private bool blockRaycasts = true;

        [SerializeField]
        [Tooltip("点击遮罩进入下一步")]
        private bool clickMaskToNext = false;

        private TutorialHighlightUI highlightUI;
        private RectTransform resolvedTarget;

        public void SetTarget(RectTransform target)
        {
            targetUI = target;
            resolvedTarget = target;
        }

        protected override void OnActivate()
        {
            ResolveTarget();
            if (resolvedTarget == null)
            {
                Debug.LogWarning("[HighlightModule] No target found!");
                return;
            }

            highlightUI = TutorialUIPool.GetHighlight();
            if (highlightUI != null)
            {
                var canvas = context?.TargetCanvas;
                if (canvas == null)
                    canvas = UnityEngine.Object.FindFirstObjectByType<Canvas>();

                highlightUI.transform.SetParent(canvas?.transform, false);
                highlightUI.Setup(resolvedTarget, highlightType, shape,
                    maskColor, highlightColor, padding, cornerRadius,
                    enablePulseAnimation, pulseSpeed, pulseAmplitude,
                    blockRaycasts, clickMaskToNext, canvas);

                if (clickMaskToNext)
                    highlightUI.OnMaskClicked += HandleMaskClick;

                highlightUI.Show();
            }
        }

        protected override void OnDeactivate()
        {
            if (highlightUI != null)
            {
                if (clickMaskToNext)
                    highlightUI.OnMaskClicked -= HandleMaskClick;

                TutorialUIPool.ReturnHighlight(highlightUI);
                highlightUI = null;
            }
        }

        public override void UpdateModule()
        {
            if (isActive && highlightUI != null)
                highlightUI.UpdateHighlight();
        }

        private void ResolveTarget()
        {
            if (targetUI != null)
            {
                resolvedTarget = targetUI;
                return;
            }

            if (!string.IsNullOrEmpty(targetPath))
            {
                var go = GameObject.Find(targetPath);
                if (go != null)
                    resolvedTarget = go.GetComponent<RectTransform>();
            }
        }

        private void HandleMaskClick()
        {
            TutorialManager.Instance?.NextStep();
        }
    }
}
