using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TutorialSystem
{
    /// <summary>
    /// 教程阶段（包含多个步骤）
    /// </summary>
    [Serializable]
    public class TutorialPhase
    {
        public string phaseName;

        public List<TutorialStep> steps = new List<TutorialStep>();

        [NonSerialized]
        public TutorialPhaseEvents events = new TutorialPhaseEvents();

        /// <summary>
        /// 获取第一个步骤索引
        /// </summary>
        public int GetFirstStepIndex()
        {
            if (steps.Count > 0) return 0;
            return -1;
        }

        /// <summary>
        /// 获取下一个步骤索引
        /// </summary>
        public int GetNextStepIndex(int currentIndex)
        {
            if (currentIndex + 1 < steps.Count) return currentIndex + 1;
            return -1;
        }

        /// <summary>
        /// 获取上一个步骤索引
        /// </summary>
        public int GetPrevStepIndex(int currentIndex)
        {
            if (currentIndex - 1 >= 0) return currentIndex - 1;
            return -1;
        }
    }

    public class TutorialPhaseEvents
    {
        [Tooltip("Triggered when entering phase")]
        public UnityEvent onEnter = new UnityEvent();
        
        [Tooltip("Triggered when phase complete")]
        public UnityEvent onComplete = new UnityEvent();
        
        [Tooltip("Triggered when exiting phase")]
        public UnityEvent onExit = new UnityEvent();
    }
}
