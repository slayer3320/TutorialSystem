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

        public virtual void Initialize(TutorialContext context)
        {
            this.context = context;
            isTriggered = false;
        }

        public virtual void Enable()
        {
            isTriggered = false;
        }

        public virtual void Disable()
        {
        }

        public virtual void Reset()
        {
            isTriggered = false;
        }

        public virtual void Update() { }

        protected void Trigger()
        {
            if (isTriggered) return;
            isTriggered = true;
            OnTriggered?.Invoke();
        }
    }
}
