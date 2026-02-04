using System;
using System.Collections.Generic;
using UnityEngine;

namespace TutorialSystem
{
    /// <summary>
    /// 模块位置模式
    /// </summary>
    public enum ModulePositionMode
    {
        /// <summary>
        /// 基于Transform/RectTransform定位
        /// </summary>
        TransformBased,
        /// <summary>
        /// 手动设置位置
        /// </summary>
        Manual
    }

    /// <summary>
    /// 目标空间类型
    /// </summary>
    public enum TargetSpaceType
    {
        /// <summary>
        /// Canvas空间（RectTransform）
        /// </summary>
        CanvasSpace,
        /// <summary>
        /// 世界空间（Transform）
        /// </summary>
        WorldSpace
    }

    /// <summary>
    /// 放置类型
    /// </summary>
    public enum PlacementType
    {
        Center,
        Top,
        Bottom,
        Left,
        Right,
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight
    }

    /// <summary>
    /// 模块尺寸类型
    /// </summary>
    public enum ModuleSizeType
    {
        Auto,
        CustomSize
    }

    /// <summary>
    /// 模块基类
    /// </summary>
    [Serializable]
    public abstract class TutorialModuleBase : ITutorialModule
    {
        public abstract string ModuleName { get; }

        #region Base Properties
        
        [SerializeField]
        [Tooltip("是否每帧更新位置")]
        protected bool updateEveryFrame = false;

        [SerializeField]
        [Tooltip("目标Canvas")]
        protected Canvas targetCanvas;

        [SerializeField]
        [Tooltip("位置模式")]
        protected ModulePositionMode positionMode = ModulePositionMode.TransformBased;

        [SerializeField]
        [Tooltip("目标空间类型")]
        protected TargetSpaceType targetType = TargetSpaceType.CanvasSpace;

        [SerializeField]
        [Tooltip("目标Transform")]
        protected Transform target;

        [SerializeField]
        [Tooltip("放置类型")]
        protected PlacementType placementType = PlacementType.Center;

        [SerializeField]
        [Tooltip("是否限制在Canvas范围内")]
        protected bool constrainToCanvas = false;

        [SerializeField]
        [Tooltip("位置偏移")]
        protected Vector2 positionOffset = Vector2.zero;

        [SerializeField]
        [Tooltip("手动位置（Manual模式使用）")]
        protected Vector2 manualPosition = Vector2.zero;

        #endregion

        #region Module Properties

        [SerializeField]
        [Tooltip("尺寸类型")]
        protected ModuleSizeType sizeType = ModuleSizeType.Auto;

        [SerializeField]
        [Tooltip("自定义尺寸")]
        protected Vector2 customSize = new Vector2(200, 100);

        #endregion

        #region Effects

        [SerializeField]
        [Tooltip("效果设置")]
        protected EffectSettings effectSettings = new EffectSettings();

        #endregion

        protected bool isActive;
        public bool IsActive => isActive;

        protected TutorialContext context;

        // 运行时Effect列表
        protected List<IEffect> runtimeEffects = new List<IEffect>();

        #region Properties Accessors

        public Canvas TargetCanvas
        {
            get => targetCanvas;
            set => targetCanvas = value;
        }

        public ModulePositionMode PositionMode
        {
            get => positionMode;
            set => positionMode = value;
        }

        public Transform Target
        {
            get => target;
            set => target = value;
        }

        public RectTransform TargetRectTransform => target as RectTransform;

        #endregion

        #region Public Methods

        /// <summary>
        /// 设置目标Transform
        /// </summary>
        public void SetTarget(Transform target)
        {
            this.target = target;
        }

        #endregion

        public virtual void Initialize(TutorialContext context)
        {
            this.context = context;
            isActive = false;
            
            // 如果没有指定Canvas，尝试从context获取
            if (targetCanvas == null && context?.TargetCanvas != null)
            {
                targetCanvas = context.TargetCanvas;
            }
        }

        public virtual void Activate()
        {
            isActive = true;
            OnActivate();
        }

        public virtual void Deactivate()
        {
            isActive = false;

            // 停止所有Effect
            StopAllEffects();

            OnDeactivate();
        }

        public virtual void UpdateModule()
        {
            // 更新所有Effect
            foreach (var effect in runtimeEffects)
            {
                effect?.Update();
            }
        }

        /// <summary>
        /// 初始化并启动所有配置的Effect
        /// </summary>
        protected void InitializeAndPlayEffects(RectTransform target)
        {
            if (target == null) return;
            effectSettings.InitializeAndPlay(target, runtimeEffects);
        }

        /// <summary>
        /// 停止所有Effect
        /// </summary>
        protected void StopAllEffects()
        {
            effectSettings.StopAll(runtimeEffects);
        }

        /// <summary>
        /// 计算模块在Canvas上的位置
        /// </summary>
        protected Vector2 CalculatePosition(RectTransform moduleRect)
        {
            if (positionMode == ModulePositionMode.Manual)
            {
                return manualPosition + positionOffset;
            }

            if (target == null) return Vector2.zero;

            Vector2 targetPosition;

            if (targetType == TargetSpaceType.CanvasSpace && target is RectTransform targetRect)
            {
                targetPosition = GetRectTransformCanvasPosition(targetRect, moduleRect);
            }
            else
            {
                targetPosition = WorldToCanvasPosition(target.position, moduleRect);
            }

            // 应用放置类型偏移
            targetPosition += GetPlacementOffset(moduleRect);
            
            // 应用自定义偏移
            targetPosition += positionOffset;

            // 约束到Canvas范围
            if (constrainToCanvas && targetCanvas != null)
            {
                targetPosition = ConstrainToCanvas(targetPosition, moduleRect);
            }

            return targetPosition;
        }

        /// <summary>
        /// 获取RectTransform在Canvas上的位置
        /// </summary>
        private Vector2 GetRectTransformCanvasPosition(RectTransform targetRect, RectTransform moduleRect)
        {
            if (targetCanvas == null) return Vector2.zero;

            var canvasRect = targetCanvas.GetComponent<RectTransform>();
            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(targetCanvas.worldCamera, targetRect.position);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPoint, targetCanvas.worldCamera, out Vector2 localPoint);
            
            return localPoint;
        }

        /// <summary>
        /// 世界坐标转Canvas坐标
        /// </summary>
        private Vector2 WorldToCanvasPosition(Vector3 worldPosition, RectTransform moduleRect)
        {
            if (targetCanvas == null) return Vector2.zero;

            var canvasRect = targetCanvas.GetComponent<RectTransform>();
            var camera = Camera.main;
            
            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(camera, worldPosition);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPoint, targetCanvas.worldCamera, out Vector2 localPoint);
            
            return localPoint;
        }

        /// <summary>
        /// 根据放置类型计算偏移
        /// </summary>
        private Vector2 GetPlacementOffset(RectTransform moduleRect)
        {
            if (target == null || moduleRect == null) return Vector2.zero;

            Vector2 targetSize = Vector2.zero;
            Vector2 moduleSize = moduleRect.sizeDelta;

            if (target is RectTransform targetRect)
            {
                targetSize = targetRect.sizeDelta;
            }

            float halfTargetWidth = targetSize.x / 2;
            float halfTargetHeight = targetSize.y / 2;
            float halfModuleWidth = moduleSize.x / 2;
            float halfModuleHeight = moduleSize.y / 2;

            return placementType switch
            {
                PlacementType.Top => new Vector2(0, halfTargetHeight + halfModuleHeight),
                PlacementType.Bottom => new Vector2(0, -halfTargetHeight - halfModuleHeight),
                PlacementType.Left => new Vector2(-halfTargetWidth - halfModuleWidth, 0),
                PlacementType.Right => new Vector2(halfTargetWidth + halfModuleWidth, 0),
                PlacementType.TopLeft => new Vector2(-halfTargetWidth - halfModuleWidth, halfTargetHeight + halfModuleHeight),
                PlacementType.TopRight => new Vector2(halfTargetWidth + halfModuleWidth, halfTargetHeight + halfModuleHeight),
                PlacementType.BottomLeft => new Vector2(-halfTargetWidth - halfModuleWidth, -halfTargetHeight - halfModuleHeight),
                PlacementType.BottomRight => new Vector2(halfTargetWidth + halfModuleWidth, -halfTargetHeight - halfModuleHeight),
                _ => Vector2.zero
            };
        }

        /// <summary>
        /// 约束位置到Canvas范围内
        /// </summary>
        private Vector2 ConstrainToCanvas(Vector2 position, RectTransform moduleRect)
        {
            if (targetCanvas == null || moduleRect == null) return position;

            var canvasRect = targetCanvas.GetComponent<RectTransform>();
            Vector2 canvasSize = canvasRect.sizeDelta;
            Vector2 moduleSize = moduleRect.sizeDelta;

            float halfWidth = moduleSize.x / 2;
            float halfHeight = moduleSize.y / 2;
            float maxX = canvasSize.x / 2 - halfWidth;
            float maxY = canvasSize.y / 2 - halfHeight;

            position.x = Mathf.Clamp(position.x, -maxX, maxX);
            position.y = Mathf.Clamp(position.y, -maxY, maxY);

            return position;
        }

        protected abstract void OnActivate();
        protected abstract void OnDeactivate();
    }
}
