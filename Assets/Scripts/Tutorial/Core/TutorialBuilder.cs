using System;
using UnityEngine;
using UnityEngine.UI;

namespace TutorialSystem
{
    /// <summary>
    /// 教程构建器 - 用于在运行时动态构建教程
    /// </summary>
    public class TutorialBuilder
    {
        private TutorialConfig config;
        private TutorialPhase currentPhase;
        private TutorialStep currentStep;

        public TutorialBuilder(string tutorialName)
        {
            config = new TutorialConfig
            {
                tutorialName = tutorialName,
                tutorialId = Guid.NewGuid().ToString()
            };
        }

        public TutorialBuilder AddPhase(string phaseName)
        {
            currentPhase = new TutorialPhase
            {
                phaseId = Guid.NewGuid().ToString(),
                phaseName = phaseName,
                enabled = true
            };
            config.phases.Add(currentPhase);
            return this;
        }

        public TutorialBuilder AddStep(string stepName)
        {
            if (currentPhase == null) AddPhase("Default Phase");

            currentStep = new TutorialStep
            {
                stepId = Guid.NewGuid().ToString(),
                stepName = stepName,
                enabled = true
            };
            currentPhase.steps.Add(currentStep);
            return this;
        }

        public TutorialBuilder WithModule(ITutorialModule module)
        {
            currentStep?.modules.Add(module);
            return this;
        }

        public TutorialBuilder WithTrigger(ITutorialTrigger trigger)
        {
            if (currentStep != null) currentStep.completeTrigger = trigger;
            return this;
        }

        public TutorialBuilder WithPauseOnEnter(bool pause = true)
        {
            if (currentStep != null)
            {
                currentStep.pauseOnEnter = pause;
                currentStep.resumeOnExit = pause;
            }
            return this;
        }

        public TutorialBuilder WithPopup(string title, string content)
        {
            var popup = new PopupModule();
            popup.SetContent(title, content);
            return WithModule(popup);
        }

        public TutorialBuilder WithArrow(RectTransform target, ArrowDirection direction = ArrowDirection.Down)
        {
            var arrow = new ArrowModule();
            arrow.SetTarget(target);
            return WithModule(arrow);
        }

        public TutorialBuilder WithHighlight(RectTransform target)
        {
            var highlight = new HighlightModule();
            highlight.SetTarget(target);
            return WithModule(highlight);
        }

        public TutorialBuilder WithTimerTrigger(float delay)
        {
            var trigger = new TimerTrigger();
            trigger.SetDelay(delay);
            return WithTrigger(trigger);
        }

        public TutorialBuilder WithButtonClickTrigger(Button button)
        {
            var trigger = new ButtonClickTrigger();
            trigger.SetTargetButton(button);
            return WithTrigger(trigger);
        }

        public TutorialBuilder WithManualTrigger()
        {
            return WithTrigger(new ManualTrigger());
        }

        public TutorialBuilder WithKeyTrigger(KeyCode key)
        {
            return WithTrigger(new KeyPressTrigger());
        }

        public TutorialBuilder CanSkip(bool canSkip = true)
        {
            config.canSkip = canSkip;
            return this;
        }

        public TutorialBuilder SaveOnComplete(bool save = true)
        {
            config.saveProgressOnComplete = save;
            return this;
        }

        public TutorialConfig Build() => config;

        public void BuildAndStart()
        {
            TutorialManager.Instance?.StartTutorial(Build());
        }
    }
}
