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
            if (!isEnabled || isTriggered) return;
            
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= delay)
            {
                Trigger();
            }
        }
    }
}
