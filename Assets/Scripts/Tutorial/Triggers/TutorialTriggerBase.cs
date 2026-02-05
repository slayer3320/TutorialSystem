using System;
using UnityEngine;

namespace TutorialSystem
{
    /// <summary>
    /// 触发器基类
    /// </summary>
    [Serializable]
    public abstract class TutorialTriggerBase : ITutorialTrigger
    {
        public abstract string TriggerName { get; }

        protected bool isTriggered;
        public bool IsTriggered => isTriggered;

        protected TutorialContext context;

        public event Action OnTriggered;

        [SerializeField]
        [Tooltip("触发后的延迟时间（秒），0表示立即触发")]
        protected float delayTime = 0f;

        private bool isWaitingForDelay;
        private float delayElapsedTime;

        public virtual void Initialize(TutorialContext context)
        {
            this.context = context;
            isTriggered = false;
            isWaitingForDelay = false;
            delayElapsedTime = 0f;
        }

        public virtual void Enable()
        {
            isTriggered = false;
            isWaitingForDelay = false;
            delayElapsedTime = 0f;
        }

        public virtual void Disable()
        {
        }

        public virtual void Reset()
        {
            isTriggered = false;
            isWaitingForDelay = false;
            delayElapsedTime = 0f;
        }

        public virtual void Update()
        {
            if (isWaitingForDelay)
            {
                delayElapsedTime += Time.deltaTime;
                if (delayElapsedTime >= delayTime)
                {
                    CompleteTrigger();
                }
            }
        }

        /// <summary>
        /// 开始触发流程（会考虑延迟时间）
        /// </summary>
        protected void Trigger()
        {
            if (isTriggered || isWaitingForDelay) return;

            if (delayTime > 0)
            {
                isWaitingForDelay = true;
                delayElapsedTime = 0f;
            }
            else
            {
                CompleteTrigger();
            }
        }

        /// <summary>
        /// 完成触发（延迟时间结束后调用）
        /// </summary>
        protected void CompleteTrigger()
        {
            isTriggered = true;
            isWaitingForDelay = false;
            OnTriggered?.Invoke();
        }
    }
}
