using System;
using UnityEngine;

namespace TutorialSystem
{
    /// <summary>
    /// Sprite模块 - 用于显示框选UI高亮提示
    /// </summary>
    [Serializable]
    public class SpriteModule : TutorialModuleBase
    {
        public override string ModuleName => "Sprite";

        #region Sprite Properties

        [SerializeField]
        [Tooltip("显示的Sprite图片")]
        private Sprite sprite;

        [SerializeField]
        [Tooltip("Sprite颜色")]
        private Color color = Color.white;

        [SerializeField]
        [Tooltip("是否保持Sprite原始比例")]
        private bool preserveAspect = true;

        #endregion

        private TutorialSpriteUI spriteUI;

        public Sprite Sprite
        {
            get => sprite;
            set => sprite = value;
        }

        public Color Color
        {
            get => color;
            set => color = value;
        }

        public bool PreserveAspect
        {
            get => preserveAspect;
            set => preserveAspect = value;
        }

        protected override void OnActivate()
        {
            spriteUI = TutorialUIPool.GetSprite();
            if (spriteUI != null)
            {
                // 设置Canvas
                if (targetCanvas != null)
                {
                    spriteUI.transform.SetParent(targetCanvas.transform, false);
                }

                // 设置尺寸
                Vector2 size = sizeType == ModuleSizeType.CustomSize ? customSize : Vector2.zero;

                spriteUI.Setup(this, sprite, color, size, preserveAspect);

                // 初始化并播放所有配置的Effect
                InitializeAndPlayEffects(spriteUI.RectTransform);

                spriteUI.Show();
            }
        }

        protected override void OnDeactivate()
        {
            if (spriteUI != null)
            {
                spriteUI.Hide();
                TutorialUIPool.ReturnSprite(spriteUI);
                spriteUI = null;
            }
        }

        public override void UpdateModule()
        {
            if (spriteUI != null && isActive && !isDeactivating)
            {
                // 先计算目标位置
                Vector2 targetPos = GetTargetPosition(spriteUI.RectTransform);

                // 更新Effect的基础位置（如果有FloatingEffect）
                foreach (var effect in runtimeEffects)
                {
                    if (effect is FloatingEffect floatingEffect)
                    {
                        floatingEffect.UpdateBasePosition(targetPos);
                    }
                }

                // 应用位置
                spriteUI.RectTransform.anchoredPosition = targetPos;
            }

            base.UpdateModule();
        }

        protected override RectTransform GetEffectTargetRectTransform()
        {
            if (spriteUI == null) return null;
            return spriteUI.RectTransform;
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
