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

        #region Highlight Properties

        [SerializeField]
        [Tooltip("边距")]
        private Vector2 padding = new Vector2(10f, 10f);

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

        [SerializeField]
        [Tooltip("阻挡非高亮区域点击")]
        private bool blockRaycasts = true;

        [SerializeField]
        [Tooltip("点击遮罩进入下一步")]
        private bool clickMaskToNext = false;

        #endregion

        private TutorialHighlightUI highlightUI;

        protected override void OnActivate()
        {
            // 使用基类的target属性
            RectTransform targetRect = target as RectTransform;
            
            if (targetRect == null && positionMode == ModulePositionMode.TransformBased)
            {
                Debug.LogWarning("[HighlightModule] No target found!");
                return;
            }

            highlightUI = TutorialUIPool.GetHighlight();
            if (highlightUI != null)
            {
                var canvas = targetCanvas;
                if (canvas == null)
                    canvas = context?.TargetCanvas;
                if (canvas == null)
                    canvas = UnityEngine.Object.FindFirstObjectByType<Canvas>();

                highlightUI.transform.SetParent(canvas?.transform, false);
                highlightUI.Setup(targetRect, highlightType, shape,
                    maskColor, highlightColor, padding, cornerRadius,
                    blockRaycasts, clickMaskToNext, canvas);

                if (clickMaskToNext)
                    highlightUI.OnMaskClicked += HandleMaskClick;

                // 初始化并播放所有配置的Effect
                if (highlightUI.BorderRect != null)
                {
                    InitializeAndPlayEffects(highlightUI.BorderRect);
                }

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
            base.UpdateModule();

            if (isActive && highlightUI != null)
                highlightUI.UpdateHighlight();
        }

        private void HandleMaskClick()
        {
            TutorialManager.Instance?.NextStep();
        }
    }
}
