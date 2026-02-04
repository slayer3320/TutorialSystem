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
        public string tutorialName;

        public string localizationTableName = "Tutorial";

        public List<TutorialPhase> phases = new List<TutorialPhase>();

        [NonSerialized]
        public TutorialConfigEvents events = new TutorialConfigEvents();

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
    }

    public class TutorialConfigEvents
    {
        public UnityEvent onStart = new UnityEvent();
        public UnityEvent onComplete = new UnityEvent();
        public UnityEvent onSkip = new UnityEvent();
    }
}
