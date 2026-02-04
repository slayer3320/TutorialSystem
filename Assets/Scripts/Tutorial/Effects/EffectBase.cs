using System;
using UnityEngine;

namespace TutorialSystem
{
    /// <summary>
    /// Effect基类
    /// </summary>
    [Serializable]
    public abstract class EffectBase : IEffect
    {
        [SerializeField]
        protected bool enabled = true;
        public bool Enabled
        {
            get => enabled;
            set => enabled = value;
        }

        protected bool isPlaying;
        public bool IsPlaying => isPlaying;

        protected RectTransform target;

        public virtual void Initialize(RectTransform target)
        {
            this.target = target;
            isPlaying = false;
        }

        public virtual void Play()
        {
            if (!enabled || target == null) return;
            isPlaying = true;
            OnPlay();
        }

        public virtual void Stop()
        {
            isPlaying = false;
            OnStop();
        }

        public virtual void Update()
        {
            if (!enabled || !isPlaying || target == null) return;
            OnUpdate();
        }

        public virtual void Reset()
        {
            isPlaying = false;
            OnReset();
        }

        protected abstract void OnPlay();
        protected abstract void OnStop();
        protected abstract void OnUpdate();
        protected virtual void OnReset() { }
    }
}
