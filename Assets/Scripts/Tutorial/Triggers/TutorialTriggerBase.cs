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
        [Tooltip("激活延迟（秒）：步骤开始后，等待这段时间才开始监听触发条件，用于防止用户过早触发")]
        protected float activationDelay = 0f;

        [SerializeField]
        [Tooltip("触发后延迟（秒）：触发条件满足后，等待这段时间才真正执行触发动作")]
        protected float delayTime = 0f;

        private bool isActivated;
        private float activationElapsedTime;

        private bool isWaitingForDelay;
        private float delayElapsedTime;

        public virtual void Initialize(TutorialContext context)
        {
            this.context = context;
            isTriggered = false;
            isActivated = false;
            activationElapsedTime = 0f;
            isWaitingForDelay = false;
            delayElapsedTime = 0f;
        }

        public virtual void Enable()
        {
            isTriggered = false;
            isActivated = activationDelay <= 0f;
            activationElapsedTime = 0f;
            isWaitingForDelay = false;
            delayElapsedTime = 0f;
        }

        public virtual void Disable()
        {
        }

        public virtual void Reset()
        {
            isTriggered = false;
            isActivated = false;
            activationElapsedTime = 0f;
            isWaitingForDelay = false;
            delayElapsedTime = 0f;
        }

        public virtual void Update()
        {
            // 处理激活延迟
            if (!isActivated)
            {
                activationElapsedTime += Time.deltaTime;
                if (activationElapsedTime >= activationDelay)
                {
                    isActivated = true;
                }
                return;
            }

            // 处理触发后延迟
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
        /// 检查触发器是否已激活（激活延迟已过）
        /// </summary>
        protected bool IsActivated => isActivated;

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
