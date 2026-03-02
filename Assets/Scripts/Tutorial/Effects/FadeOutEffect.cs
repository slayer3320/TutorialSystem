using System;
using UnityEngine;

namespace TutorialSystem
{
    /// <summary>
    /// 渐出动画效果
    /// </summary>
    [Serializable]
    public class FadeOutEffect : EffectBase
    {
        [SerializeField]
        [Tooltip("渐出持续时间")]
        private float duration = 0.1f;

        [SerializeField]
        [Tooltip("起始透明度")]
        private float startAlpha = 1f;

        [SerializeField]
        [Tooltip("目标透明度")]
        private float endAlpha = 0f;

        private float time;
        private CanvasGroup canvasGroup;

        public float Duration
        {
            get => duration;
            set => duration = Mathf.Max(0.01f, value);
        }

        public override void Initialize(RectTransform target)
        {
            base.Initialize(target);
            if (target != null)
            {
                canvasGroup = target.GetComponent<CanvasGroup>();
            }
        }

        protected override void OnPlay()
        {
            time = 0f;
            if (canvasGroup != null)
            {
                canvasGroup.alpha = startAlpha;
            }
        }

        protected override void OnStop()
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = endAlpha;
            }
            isPlaying = false;
        }

        protected override void OnUpdate()
        {
            if (canvasGroup == null) return;

            time += Time.unscaledDeltaTime;
            float progress = Mathf.Clamp01(time / duration);
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, progress);

            if (progress >= 1f)
            {
                isPlaying = false;
            }
        }

        protected override void OnReset()
        {
            time = 0f;
            if (canvasGroup != null)
            {
                canvasGroup.alpha = startAlpha;
            }
        }
    }
}
