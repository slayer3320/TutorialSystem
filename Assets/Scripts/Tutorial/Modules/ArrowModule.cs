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
    /// 位置模式
    /// </summary>
    public enum PositionMode
    {
        /// <summary>
        /// 跟随目标 RectTransform
        /// </summary>
        FollowTarget,
        /// <summary>
        /// 固定位置
        /// </summary>
        Fixed,
        /// <summary>
        /// 跟随世界坐标物体
        /// </summary>
        WorldPosition
    }

    /// <summary>
    /// 箭头模块
    /// </summary>
    [Serializable]
    public class ArrowModule : TutorialModuleBase
    {
        public override string ModuleName => "箭头";

        [SerializeField]
        private PositionMode positionMode = PositionMode.FollowTarget;
        
        [SerializeField]
        [Tooltip("目标 UI 元素（PositionMode.FollowTarget 时使用）")]
        private RectTransform targetUI;
        
        [SerializeField]
        [Tooltip("目标路径（运行时查找，如果 targetUI 为空）")]
        private string targetPath;
        
        [SerializeField]
        [Tooltip("世界坐标目标（PositionMode.WorldPosition 时使用）")]
        private Transform worldTarget;
        
        [SerializeField]
        [Tooltip("固定位置（PositionMode.Fixed 时使用）")]
        private Vector2 fixedPosition;
        
        [SerializeField]
        [Tooltip("位置偏移")]
        private Vector2 offset;

        [SerializeField]
        private ArrowDirection direction = ArrowDirection.Down;
        
        [SerializeField]
        private Color color = Color.white;
        
        [SerializeField]
        [Tooltip("箭头大小缩放")]
        private float scale = 1f;

        [SerializeField]
        [Tooltip("浮动效果")]
        private FloatingEffect floatingEffect = new FloatingEffect();

        private TutorialArrowUI arrowUI;
        private RectTransform resolvedTarget;

        public void SetTarget(RectTransform target)
        {
            targetUI = target;
            resolvedTarget = target;
        }

        public void SetWorldTarget(Transform target)
        {
            worldTarget = target;
            positionMode = PositionMode.WorldPosition;
        }

        protected override void OnActivate()
        {
            ResolveTarget();
            
            arrowUI = TutorialUIPool.GetArrow();
            if (arrowUI != null)
            {
                // 设置浮动方向
                floatingEffect.Direction = GetFloatDirection();
                
                arrowUI.Setup(this, resolvedTarget, worldTarget, positionMode, direction, 
                    offset, color, scale, fixedPosition);
                
                // 注册Effect
                RegisterEffect(floatingEffect, arrowUI.RectTransform);
                
                arrowUI.Show();
            }
        }

        protected override void OnDeactivate()
        {
            ClearEffects();
            
            if (arrowUI != null)
            {
                arrowUI.Hide();
                TutorialUIPool.ReturnArrow(arrowUI);
                arrowUI = null;
            }
        }

        public override void UpdateModule()
        {
            base.UpdateModule();
            
            if (arrowUI != null && isActive)
            {
                arrowUI.UpdatePosition();
                
                // 更新浮动效果基础位置
                if (floatingEffect.Enabled && floatingEffect.IsPlaying)
                {
                    floatingEffect.UpdateBasePosition(arrowUI.RectTransform.anchoredPosition);
                }
            }
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
                {
                    resolvedTarget = go.GetComponent<RectTransform>();
                }
            }
        }

        private Vector2 GetFloatDirection()
        {
            return direction switch
            {
                ArrowDirection.Up or ArrowDirection.Down => Vector2.up,
                ArrowDirection.Left or ArrowDirection.Right => Vector2.right,
                _ => new Vector2(0.707f, 0.707f)
            };
        }
    }
}
