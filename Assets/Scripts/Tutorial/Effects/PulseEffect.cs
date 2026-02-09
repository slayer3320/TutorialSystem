using System;
using UnityEngine;
using UnityEngine.UI;

namespace TutorialSystem
{
    /// <summary>
    /// 脉冲动画效果
    /// </summary>
    [Serializable]
    public class PulseEffect : EffectBase
    {
        [SerializeField]
        [Tooltip("脉冲速度")]
        private float speed = 2f;

        [SerializeField]
        [Tooltip("缩放幅度")]
        private float scaleAmplitude = 0.1f;

        [SerializeField]
        [Tooltip("透明度幅度")]
        private float alphaAmplitude = 0.2f;

        [SerializeField]
        [Tooltip("基础透明度")]
        private float baseAlpha = 0.3f;

        private float time;
        private Vector3 originalScale;
        private Graphic targetGraphic;

        public float Speed
        {
            get => speed;
            set => speed = value;
        }

        public float ScaleAmplitude
        {
            get => scaleAmplitude;
            set => scaleAmplitude = value;
        }

        public float AlphaAmplitude
        {
            get => alphaAmplitude;
            set => alphaAmplitude = value;
        }

        public override void Initialize(RectTransform target)
        {
            base.Initialize(target);
            if (target != null)
            {
                originalScale = target.localScale;
                targetGraphic = target.GetComponent<Graphic>();
            }
        }

        protected override void OnPlay()
        {
            time = 0f;
        }

        protected override void OnStop()
        {
            if (target != null)
            {
                target.localScale = originalScale;
            }
            if (targetGraphic != null)
            {
                var color = targetGraphic.color;
                color.a = baseAlpha;
                targetGraphic.color = color;
            }
        }

        protected override void OnUpdate()
        {
            time += Time.unscaledDeltaTime * speed;

            // 缩放动画
            float scale = 1f + Mathf.Sin(time) * scaleAmplitude;
            target.localScale = originalScale * scale;

            // 透明度动画
            if (targetGraphic != null)
            {
                float alpha = baseAlpha + Mathf.Sin(time) * alphaAmplitude;
                var color = targetGraphic.color;
                color.a = alpha;
                targetGraphic.color = color;
            }
        }

        protected override void OnReset()
        {
            time = 0f;
            if (target != null)
            {
                target.localScale = originalScale;
            }
        }
    }
}
