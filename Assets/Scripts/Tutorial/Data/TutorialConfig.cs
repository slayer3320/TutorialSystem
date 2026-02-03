using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TutorialSystem
{
    /// <summary>
    /// 教程配置数据
    /// </summary>
    [Serializable]
    public class TutorialConfig
    {
        [Header("基本信息")]
        public string tutorialId;
        public string tutorialName;

        [Header("本地化")]
        public string localizationTableName = "Tutorial";

        [Header("阶段列表")]
        public List<TutorialPhase> phases = new List<TutorialPhase>();

        [Header("事件")]
        public TutorialConfigEvents events = new TutorialConfigEvents();

        [Header("设置")]
        public bool saveProgressOnComplete = true;
        public bool canSkip = false;

        public int GetFirstEnabledPhaseIndex()
        {
            for (int i = 0; i < phases.Count; i++)
                if (phases[i].enabled) return i;
            return -1;
        }

        public int GetNextEnabledPhaseIndex(int currentIndex)
        {
            for (int i = currentIndex + 1; i < phases.Count; i++)
                if (phases[i].enabled) return i;
            return -1;
        }

        public int GetPrevEnabledPhaseIndex(int currentIndex)
        {
            for (int i = currentIndex - 1; i >= 0; i--)
                if (phases[i].enabled) return i;
            return -1;
        }

        public int GetTotalStepCount()
        {
            int count = 0;
            foreach (var phase in phases)
            {
                if (!phase.enabled) continue;
                foreach (var step in phase.steps)
                    if (step.enabled) count++;
            }
            return count;
        }

#if UNITY_EDITOR
        public void GenerateIds()
        {
            if (string.IsNullOrEmpty(tutorialId))
                tutorialId = Guid.NewGuid().ToString();

            foreach (var phase in phases)
            {
                if (string.IsNullOrEmpty(phase.phaseId))
                    phase.phaseId = Guid.NewGuid().ToString();

                foreach (var step in phase.steps)
                {
                    if (string.IsNullOrEmpty(step.stepId))
                        step.stepId = Guid.NewGuid().ToString();
                }
            }
        }
#endif
    }

    [Serializable]
    public class TutorialConfigEvents
    {
        public UnityEvent onStart = new UnityEvent();
        public UnityEvent onComplete = new UnityEvent();
        public UnityEvent onSkip = new UnityEvent();
    }
}
