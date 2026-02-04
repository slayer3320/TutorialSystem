using System.Collections.Generic;
using UnityEngine;

namespace TutorialSystem
{
    /// <summary>
    /// 教程管理器
    /// </summary>
    public class TutorialManager : MonoBehaviour
    {
        public static TutorialManager Instance { get; private set; }

        [SerializeField] private GameObject arrowPrefab;
        [SerializeField] private GameObject popupPrefab;
        
        [SerializeField] private Transform uiContainer;
        [SerializeField] private Canvas targetCanvas;

        [SerializeField]
        [Tooltip("启用本地化支持，关闭后将使用原始文本")]
        private bool useLocalization = false;

        [SerializeField] private bool debugMode = false;

        private TutorialContext currentContext;
        private bool isRunning;
        private TutorialConfig currentConfig;

        public TutorialContext CurrentContext => currentContext;
        public bool IsRunning => isRunning;
        public TutorialConfig CurrentConfig => currentConfig;
        public TutorialPhase CurrentPhase => currentContext?.CurrentPhase;
        public TutorialStep CurrentStep => currentContext?.CurrentStep;
        public int CurrentPhaseIndex => currentContext?.PhaseIndex ?? -1;
        public int CurrentStepIndex => currentContext?.StepIndex ?? -1;
        public float Progress => currentContext?.Progress ?? 0f;
        public bool UseLocalization => useLocalization;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            TutorialUIPool.Initialize(arrowPrefab, popupPrefab, uiContainer);
            TutorialUIPool.PrewarmPool();
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
                TutorialUIPool.Clear();
            }
        }

        private void Update()
        {
            if (!isRunning || currentContext == null) return;

            currentContext.CurrentStep?.completeTrigger?.Update();

            if (!isRunning || currentContext == null) return;

            if (currentContext.CurrentStep?.modules != null)
            {
                foreach (var module in currentContext.CurrentStep.modules)
                    module?.UpdateModule();
            }
        }

        #region 公共 API

        public void StartTutorial(TutorialConfig config)
        {
            if (config == null)
            {
                Debug.LogError("[TutorialManager] Config is null!");
                return;
            }

            if (isRunning) StopTutorial();

            Log($"Starting tutorial: {config.tutorialName}");

            currentConfig = config;
            currentContext = new TutorialContext(config);
            currentContext.TargetCanvas = targetCanvas;
            isRunning = true;

            config.events.onStart?.Invoke();
            TutorialEventChannel.RaiseTutorialStart(config);

            int firstPhaseIndex = config.GetFirstPhaseIndex();
            if (firstPhaseIndex >= 0)
                EnterPhase(firstPhaseIndex);
            else
            {
                Debug.LogWarning("[TutorialManager] No phases found!");
                CompleteTutorial();
            }
        }

        public void StopTutorial()
        {
            if (!isRunning) return;

            Log("Stopping tutorial");

            ExitCurrentStep();
            ExitCurrentPhase();

            isRunning = false;
            currentContext?.Reset();
            currentContext = null;
            currentConfig = null;
        }

        public void SkipTutorial()
        {
            if (!isRunning) return;
            if (currentConfig != null && !currentConfig.canSkip)
            {
                Debug.LogWarning("[TutorialManager] Cannot skip!");
                return;
            }

            Log("Skipping tutorial");

            var config = currentConfig;
            StopTutorial();

            if (config != null)
            {
                config.events.onSkip?.Invoke();
                TutorialEventChannel.RaiseTutorialSkip(config);
            }
        }

        public void NextStep()
        {
            if (!isRunning || currentContext == null) return;

            var phase = currentContext.CurrentPhase;
            if (phase == null) return;

            int nextStepIndex = phase.GetNextStepIndex(currentContext.StepIndex);
            if (nextStepIndex >= 0)
            {
                ExitCurrentStep();
                EnterStep(nextStepIndex);
            }
            else
                NextPhase();
        }

        public void PrevStep()
        {
            if (!isRunning || currentContext == null) return;

            var phase = currentContext.CurrentPhase;
            if (phase == null) return;

            int prevStepIndex = phase.GetPrevStepIndex(currentContext.StepIndex);
            if (prevStepIndex >= 0)
            {
                ExitCurrentStep();
                EnterStep(prevStepIndex);
            }
        }

        public void NextPhase()
        {
            if (!isRunning || currentContext == null) return;

            int nextPhaseIndex = currentConfig?.GetNextPhaseIndex(currentContext.PhaseIndex) ?? -1;
            
            if (nextPhaseIndex >= 0)
            {
                ExitCurrentStep();
                ExitCurrentPhase();
                EnterPhase(nextPhaseIndex);
            }
            else
                CompleteTutorial();
        }

        public void JumpToPhase(int phaseIndex)
        {
            if (!isRunning || currentContext == null) return;
            
            var phases = currentConfig?.phases;
            if (phases == null || phaseIndex < 0 || phaseIndex >= phases.Count) return;

            ExitCurrentStep();
            ExitCurrentPhase();
            EnterPhase(phaseIndex);
        }

        public void JumpToStep(int phaseIndex, int stepIndex)
        {
            if (!isRunning || currentContext == null) return;
            
            if (phaseIndex != currentContext.PhaseIndex)
                JumpToPhase(phaseIndex);

            if (stepIndex != currentContext.StepIndex)
            {
                ExitCurrentStep();
                EnterStep(stepIndex);
            }
        }

        #endregion

        #region 内部方法

        private void EnterPhase(int phaseIndex)
        {
            var phases = currentConfig?.phases;
            if (phases == null || phaseIndex < 0 || phaseIndex >= phases.Count) return;

            var phase = phases[phaseIndex];
            currentContext.PhaseIndex = phaseIndex;
            currentContext.CurrentPhase = phase;

            Log($"Entering phase: {phase.phaseName}");

            phase.events.onEnter?.Invoke();
            TutorialEventChannel.RaisePhaseEnter(phase);

            int firstStepIndex = phase.GetFirstStepIndex();
            if (firstStepIndex >= 0)
                EnterStep(firstStepIndex);
            else
                NextPhase();
        }

        private void ExitCurrentPhase()
        {
            var phase = currentContext?.CurrentPhase;
            if (phase == null) return;

            Log($"Exiting phase: {phase.phaseName}");

            phase.events.onExit?.Invoke();
            TutorialEventChannel.RaisePhaseExit(phase);

            currentContext.CurrentPhase = null;
            currentContext.PhaseIndex = -1;
        }

        private void EnterStep(int stepIndex)
        {
            var phase = currentContext.CurrentPhase;
            if (phase == null || stepIndex < 0 || stepIndex >= phase.steps.Count) return;

            var step = phase.steps[stepIndex];
            currentContext.StepIndex = stepIndex;
            currentContext.CurrentStep = step;

            Log($"Entering step: {step.stepName}");

            if (step.pauseOnEnter)
                Time.timeScale = 0f;

            step.events.onEnter?.Invoke();
            TutorialEventChannel.RaiseStepEnter(step);

            if (step.completeTrigger != null)
            {
                step.completeTrigger.Initialize(currentContext);
                step.completeTrigger.OnTriggered += OnStepTriggerActivated;
                step.completeTrigger.Enable();
            }

            if (step.modules != null)
            {
                foreach (var module in step.modules)
                {
                    if (module != null)
                    {
                        module.Initialize(currentContext);
                        module.Activate();
                    }
                }
            }
        }

        private void ExitCurrentStep()
        {
            var step = currentContext?.CurrentStep;
            if (step == null) return;

            Log($"Exiting step: {step.stepName}");

            if (step.completeTrigger != null)
            {
                step.completeTrigger.OnTriggered -= OnStepTriggerActivated;
                step.completeTrigger.Disable();
            }

            if (step.modules != null)
            {
                foreach (var module in step.modules)
                    module?.Deactivate();
            }

            if (step.resumeOnExit)
                Time.timeScale = 1f;

            step.events.onExit?.Invoke();
            TutorialEventChannel.RaiseStepExit(step);

            currentContext.CurrentStep = null;
            currentContext.StepIndex = -1;
        }

        private void OnStepTriggerActivated()
        {
            if (currentContext?.CurrentStep != null)
            {
                currentContext.CurrentStep.events.onComplete?.Invoke();
                TutorialEventChannel.RaiseStepComplete(currentContext.CurrentStep);
            }
            NextStep();
        }

        private void CompleteTutorial()
        {
            var config = currentConfig;
            
            ExitCurrentStep();
            ExitCurrentPhase();

            Log("Tutorial completed");

            if (config != null)
            {
                if (currentContext?.CurrentPhase != null)
                {
                    currentContext.CurrentPhase.events.onComplete?.Invoke();
                    TutorialEventChannel.RaisePhaseComplete(currentContext.CurrentPhase);
                }

                config.events.onComplete?.Invoke();
                TutorialEventChannel.RaiseTutorialComplete(config);
            }

            isRunning = false;
            currentContext?.Reset();
            currentContext = null;
            currentConfig = null;
        }

        private void Log(string message)
        {
            if (debugMode)
                Debug.Log($"[TutorialManager] {message}");
        }

        #endregion
    }
}
