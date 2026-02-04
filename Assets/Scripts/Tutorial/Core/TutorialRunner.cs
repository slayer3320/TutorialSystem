using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace TutorialSystem
{
    /// <summary>
    /// 教程运行器 - 在 Inspector 中配置教程
    /// </summary>
    public class TutorialRunner : MonoBehaviour
    {
        [SerializeField] private TutorialConfig config = new TutorialConfig();

        [SerializeField] private bool autoStartOnEnable = false;

        public TutorialConfig Config => config;
        public bool IsRunning => TutorialManager.Instance?.IsRunning ?? false;

        public event Action OnTutorialStarted;
        public event Action OnTutorialCompleted;
        public event Action OnTutorialSkipped;
        public event Action<TutorialPhase> OnPhaseChanged;
        public event Action<TutorialStep> OnStepChanged;

        private void OnEnable()
        {
            SubscribeEvents();
            if (autoStartOnEnable)
            {
                StartTutorial();
            }
        }

        private void OnDisable()
        {
            UnsubscribeEvents();
        }

        private void SubscribeEvents()
        {
            TutorialEventChannel.OnTutorialStart += HandleTutorialStart;
            TutorialEventChannel.OnTutorialComplete += HandleTutorialComplete;
            TutorialEventChannel.OnTutorialSkip += HandleTutorialSkip;
            TutorialEventChannel.OnPhaseEnter += HandlePhaseEnter;
            TutorialEventChannel.OnStepEnter += HandleStepEnter;
        }

        private void UnsubscribeEvents()
        {
            TutorialEventChannel.OnTutorialStart -= HandleTutorialStart;
            TutorialEventChannel.OnTutorialComplete -= HandleTutorialComplete;
            TutorialEventChannel.OnTutorialSkip -= HandleTutorialSkip;
            TutorialEventChannel.OnPhaseEnter -= HandlePhaseEnter;
            TutorialEventChannel.OnStepEnter -= HandleStepEnter;
        }

        #region 公共 API

        public void StartTutorial()
        {
            if (config == null || config.phases.Count == 0)
            {
                Debug.LogWarning("[TutorialRunner] No config!");
                return;
            }
            TutorialManager.Instance?.StartTutorial(config);
        }

        public void StartTutorial(TutorialConfig newConfig)
        {
            config = newConfig;
            StartTutorial();
        }

        public void StopTutorial() => TutorialManager.Instance?.StopTutorial();
        public void SkipTutorial() => TutorialManager.Instance?.SkipTutorial();
        public void NextStep() => TutorialManager.Instance?.NextStep();
        public void PrevStep() => TutorialManager.Instance?.PrevStep();
        public void NextPhase() => TutorialManager.Instance?.NextPhase();
        public void JumpToPhase(int phaseIndex) => TutorialManager.Instance?.JumpToPhase(phaseIndex);
        public void JumpToStep(int phaseIndex, int stepIndex) => TutorialManager.Instance?.JumpToStep(phaseIndex, stepIndex);

        public Coroutine StartTutorialDelayed(float delay)
        {
            return StartCoroutine(StartTutorialDelayedCoroutine(delay));
        }

        public Coroutine StartTutorialWhen(Func<bool> condition, float checkInterval = 0.1f)
        {
            return StartCoroutine(StartTutorialWhenCoroutine(condition, checkInterval));
        }

        #endregion

        #region 协程

        private IEnumerator StartTutorialDelayedCoroutine(float delay)
        {
            yield return new WaitForSeconds(delay);
            StartTutorial();
        }

        private IEnumerator StartTutorialWhenCoroutine(Func<bool> condition, float checkInterval)
        {
            while (!condition())
            {
                yield return new WaitForSeconds(checkInterval);
            }
            StartTutorial();
        }

        #endregion

        #region 事件处理

        private void HandleTutorialStart(TutorialConfig c)
        {
            if (c == config) OnTutorialStarted?.Invoke();
        }

        private void HandleTutorialComplete(TutorialConfig c)
        {
            if (c == config) OnTutorialCompleted?.Invoke();
        }

        private void HandleTutorialSkip(TutorialConfig c)
        {
            if (c == config) OnTutorialSkipped?.Invoke();
        }

        private void HandlePhaseEnter(TutorialPhase phase)
        {
            if (TutorialManager.Instance?.CurrentConfig == config)
                OnPhaseChanged?.Invoke(phase);
        }

        private void HandleStepEnter(TutorialStep step)
        {
            if (TutorialManager.Instance?.CurrentConfig == config)
                OnStepChanged?.Invoke(step);
        }

        #endregion
    }
}
