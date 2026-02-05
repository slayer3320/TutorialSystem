using System;
using UnityEngine;

namespace TutorialSystem
{
    /// <summary>
    /// 计时器触发器
    /// </summary>
    [Serializable]
    public class TimerTrigger : TutorialTriggerBase
    {
        public override string TriggerName => "计时器触发";

        [SerializeField]
        [Tooltip("延迟时间（秒）")]
        private float delay = 1f;

        private float elapsedTime;

        public void SetDelay(float delaySeconds)
        {
            delay = delaySeconds;
        }

        public override void Enable()
        {
            base.Enable();
            elapsedTime = 0f;
        }

        public override void Reset()
        {
            base.Reset();
            elapsedTime = 0f;
        }

        public override void Update()
        {
            if (isTriggered) return;

            elapsedTime += Time.deltaTime;
            if (elapsedTime >= delay)
            {
                // Timer触发器不使用额外的delayTime，直接完成触发
                CompleteTrigger();
            }
        }
    }
}
