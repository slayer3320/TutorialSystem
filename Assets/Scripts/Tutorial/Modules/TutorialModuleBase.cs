using System;
using System.Collections.Generic;
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
        
        // Effect列表
        protected List<IEffect> effects = new List<IEffect>();

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
            
            // 播放所有Effect
            foreach (var effect in effects)
            {
                effect.Play();
            }
        }

        public virtual void Deactivate()
        {
            isActive = false;
            
            // 停止所有Effect
            foreach (var effect in effects)
            {
                effect.Stop();
            }
            
            OnDeactivate();
        }

        public virtual void UpdateModule()
        {
            // 更新所有Effect
            foreach (var effect in effects)
            {
                effect.Update();
            }
        }

        /// <summary>
        /// 注册Effect
        /// </summary>
        protected void RegisterEffect(IEffect effect, RectTransform target)
        {
            if (effect == null || target == null) return;
            effect.Initialize(target);
            effects.Add(effect);
        }

        /// <summary>
        /// 清理所有Effect
        /// </summary>
        protected void ClearEffects()
        {
            foreach (var effect in effects)
            {
                effect.Stop();
                effect.Reset();
            }
            effects.Clear();
        }
        
        protected abstract void OnActivate();
        protected abstract void OnDeactivate();
    }
}
