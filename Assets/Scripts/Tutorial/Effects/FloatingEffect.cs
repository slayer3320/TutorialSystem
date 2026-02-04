using System;
using UnityEngine;

namespace TutorialSystem
{
    /// <summary>
    /// 浮动动画效果
    /// </summary>
    [Serializable]
    public class FloatingEffect : EffectBase
    {
        [SerializeField]
        [Tooltip("浮动幅度")]
        private float amplitude = 10f;

        [SerializeField]
        [Tooltip("浮动速度")]
        private float speed = 2f;

        [SerializeField]
        [Tooltip("浮动方向")]
        private Vector2 direction = Vector2.up;

        private float time;
        private Vector2 originalPosition;
        private bool hasOriginalPosition;

        public float Amplitude
        {
            get => amplitude;
            set => amplitude = value;
        }

        public float Speed
        {
            get => speed;
            set => speed = value;
        }

        public Vector2 Direction
        {
            get => direction;
            set => direction = value.normalized;
        }

        protected override void OnPlay()
        {
            time = 0f;
            if (!hasOriginalPosition && target != null)
            {
                originalPosition = target.anchoredPosition;
                hasOriginalPosition = true;
            }
        }

        protected override void OnStop()
        {
            if (hasOriginalPosition && target != null)
            {
                target.anchoredPosition = originalPosition;
            }
        }

        protected override void OnUpdate()
        {
            time += Time.deltaTime * speed;
            float offset = Mathf.Sin(time) * amplitude;
            target.anchoredPosition = originalPosition + direction.normalized * offset;
        }

        protected override void OnReset()
        {
            time = 0f;
            hasOriginalPosition = false;
        }

        /// <summary>
        /// 更新基础位置（供外部位置变化时调用）
        /// </summary>
        public void UpdateBasePosition(Vector2 newPosition)
        {
            originalPosition = newPosition;
            hasOriginalPosition = true;
        }

        /// <summary>
        /// 获取当前浮动偏移量
        /// </summary>
        public Vector2 GetCurrentOffset()
        {
            if (!isPlaying) return Vector2.zero;
            float offset = Mathf.Sin(time) * amplitude;
            return direction.normalized * offset;
        }
    }
}
