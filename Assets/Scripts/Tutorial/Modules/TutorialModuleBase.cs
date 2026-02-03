using System;
using UnityEngine;

namespace TutorialSystem
{
    /// <summary>
    /// 模块基类
    /// </summary>
    [Serializable]
    public abstract class TutorialModuleBase : ITutorialModule
    {
        public abstract string ModuleName { get; }
        
        [SerializeField]
        protected bool enabled = true;
        
        protected bool isActive;
        public bool IsActive => isActive;
        
        protected TutorialContext context;

        public virtual void Initialize(TutorialContext context)
        {
            this.context = context;
            isActive = false;
        }

        public virtual void Activate()
        {
            if (!enabled) return;
            isActive = true;
            OnActivate();
        }

        public virtual void Deactivate()
        {
            isActive = false;
            OnDeactivate();
        }

        public virtual void UpdateModule() { }
        
        protected abstract void OnActivate();
        protected abstract void OnDeactivate();
    }
}
