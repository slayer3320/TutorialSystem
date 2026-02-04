using System;
using System.Collections.Generic;
using UnityEngine;

namespace TutorialSystem
{
    /// <summary>
    /// Effect配置容器，包含所有可用的Effect类型及其启用状态
    /// </summary>
    [Serializable]
    public class EffectSettings
    {
        [SerializeField] private bool floatingEnabled;
        [SerializeField] private FloatingEffect floatingEffect = new FloatingEffect();

        [SerializeField] private bool fadeInEnabled;
        [SerializeField] private FadeInEffect fadeInEffect = new FadeInEffect();

        [SerializeField] private bool pulseEnabled;
        [SerializeField] private PulseEffect pulseEffect = new PulseEffect();

        public bool FloatingEnabled
        {
            get => floatingEnabled;
            set => floatingEnabled = value;
        }

        public FloatingEffect FloatingEffect => floatingEffect;

        public bool FadeInEnabled
        {
            get => fadeInEnabled;
            set => fadeInEnabled = value;
        }

        public FadeInEffect FadeInEffect => fadeInEffect;

        public bool PulseEnabled
        {
            get => pulseEnabled;
            set => pulseEnabled = value;
        }

        public PulseEffect PulseEffect => pulseEffect;

        /// <summary>
        /// 获取启用的Effect数量
        /// </summary>
        public int EnabledCount
        {
            get
            {
                int count = 0;
                if (floatingEnabled) count++;
                if (fadeInEnabled) count++;
                if (pulseEnabled) count++;
                return count;
            }
        }

        /// <summary>
        /// 获取所有启用的Effect
        /// </summary>
        public IEnumerable<IEffect> GetEnabledEffects()
        {
            if (floatingEnabled) yield return floatingEffect;
            if (fadeInEnabled) yield return fadeInEffect;
            if (pulseEnabled) yield return pulseEffect;
        }

        /// <summary>
        /// 初始化并播放所有启用的Effect
        /// </summary>
        public void InitializeAndPlay(RectTransform target, List<IEffect> runtimeList)
        {
            runtimeList.Clear();
            
            foreach (var effect in GetEnabledEffects())
            {
                effect.Initialize(target);
                effect.Play();
                runtimeList.Add(effect);
            }
        }

        /// <summary>
        /// 停止所有运行时Effect
        /// </summary>
        public void StopAll(List<IEffect> runtimeList)
        {
            foreach (var effect in runtimeList)
            {
                effect?.Stop();
                effect?.Reset();
            }
            runtimeList.Clear();
        }
    }
}
