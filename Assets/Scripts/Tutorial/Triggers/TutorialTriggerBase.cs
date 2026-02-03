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
        
        [SerializeField]
        protected bool isEnabled;
        public bool IsEnabled => isEnabled;
        
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
            isEnabled = true;
            isTriggered = false;
        }

        public virtual void Disable()
        {
            isEnabled = false;
        }

        public virtual void Reset()
        {
            isTriggered = false;
        }

        public virtual void Update() { }

        protected void Trigger()
        {
            if (!isEnabled || isTriggered) return;
            isTriggered = true;
            OnTriggered?.Invoke();
        }
    }
}
