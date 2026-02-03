using System;

namespace TutorialSystem
{
    /// <summary>
    /// 教程生命周期事件
    /// </summary>
    public static class TutorialEventChannel
    {
        // 教程级别事件
        public static event Action<TutorialConfig> OnTutorialStart;
        public static event Action<TutorialConfig> OnTutorialComplete;
        public static event Action<TutorialConfig> OnTutorialSkip;

        // 阶段级别事件
        public static event Action<TutorialPhase> OnPhaseEnter;
        public static event Action<TutorialPhase> OnPhaseComplete;
        public static event Action<TutorialPhase> OnPhaseExit;

        // 步骤级别事件
        public static event Action<TutorialStep> OnStepEnter;
        public static event Action<TutorialStep> OnStepComplete;
        public static event Action<TutorialStep> OnStepExit;

        internal static void RaiseTutorialStart(TutorialConfig config) => OnTutorialStart?.Invoke(config);
        internal static void RaiseTutorialComplete(TutorialConfig config) => OnTutorialComplete?.Invoke(config);
        internal static void RaiseTutorialSkip(TutorialConfig config) => OnTutorialSkip?.Invoke(config);
        internal static void RaisePhaseEnter(TutorialPhase phase) => OnPhaseEnter?.Invoke(phase);
        internal static void RaisePhaseComplete(TutorialPhase phase) => OnPhaseComplete?.Invoke(phase);
        internal static void RaisePhaseExit(TutorialPhase phase) => OnPhaseExit?.Invoke(phase);
        internal static void RaiseStepEnter(TutorialStep step) => OnStepEnter?.Invoke(step);
        internal static void RaiseStepComplete(TutorialStep step) => OnStepComplete?.Invoke(step);
        internal static void RaiseStepExit(TutorialStep step) => OnStepExit?.Invoke(step);
    }
}
