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

        public int GetFirstPhaseIndex()
        {
            if (phases.Count > 0) return 0;
            return -1;
        }

        public int GetNextPhaseIndex(int currentIndex)
        {
            if (currentIndex + 1 < phases.Count) return currentIndex + 1;
            return -1;
        }

        public int GetPrevPhaseIndex(int currentIndex)
        {
            if (currentIndex - 1 >= 0) return currentIndex - 1;
            return -1;
        }

        public int GetTotalStepCount()
        {
            int count = 0;
            foreach (var phase in phases)
            {
                count += phase.steps.Count;
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
