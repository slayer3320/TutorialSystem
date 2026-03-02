using System;
using UnityEngine;

namespace TutorialSystem
{
    /// <summary>
    /// 箭头方向
    /// </summary>
    public enum ArrowDirection
    {
        Up,
        Down,
        Left,
        Right,
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight
    }

    /// <summary>
    /// 箭头模块
    /// </summary>
    [Serializable]
    public class ArrowModule : TutorialModuleBase
    {
        public override string ModuleName => "Arrow";

        #region Arrow Properties

        [SerializeField]
        private ArrowDirection direction = ArrowDirection.Down;

        [SerializeField]
        private Color color = Color.white;

        [SerializeField]
        [Tooltip("箭头大小缩放")]
        private float scale = 1f;

        #endregion

        private TutorialArrowUI arrowUI;

        public ArrowDirection Direction
        {
            get => direction;
            set => direction = value;
        }

        public Color Color
        {
            get => color;
            set => color = value;
        }

        public float Scale
        {
            get => scale;
            set => scale = value;
        }

        protected override void OnActivate()
        {
            arrowUI = TutorialUIPool.GetArrow();
            if (arrowUI != null)
            {
                // 设置Canvas
                if (targetCanvas != null)
                {
                    arrowUI.transform.SetParent(targetCanvas.transform, false);
                }

                arrowUI.Setup(this, direction, color, scale);

                // 初始化并播放所有配置的Effect
                InitializeAndPlayEffects(arrowUI.RectTransform);

                arrowUI.Show();
            }
        }

        protected override void OnDeactivate()
        {
            if (arrowUI != null)
            {
                arrowUI.Hide();
                TutorialUIPool.ReturnArrow(arrowUI);
                arrowUI = null;
            }
        }

        public override void UpdateModule()
        {
            if (arrowUI != null && isActive && !isDeactivating)
            {
                // 先计算目标位置
                Vector2 targetPos = GetTargetPosition(arrowUI.RectTransform);

                // 更新Effect的基础位置（如果有FloatingEffect）
                foreach (var effect in runtimeEffects)
                {
                    if (effect is FloatingEffect floatingEffect)
                    {
                        floatingEffect.UpdateBasePosition(targetPos);
                    }
                }

                // 应用位置
                arrowUI.RectTransform.anchoredPosition = targetPos;
            }

            base.UpdateModule();
        }

        protected override RectTransform GetEffectTargetRectTransform()
        {
            if (arrowUI == null) return null;
            return arrowUI.RectTransform;
        }

        /// <summary>
        /// 获取目标位置（供UI调用）
        /// </summary>
        public Vector2 GetTargetPosition(RectTransform uiRect)
        {
            return CalculatePosition(uiRect);
        }
    }
}
