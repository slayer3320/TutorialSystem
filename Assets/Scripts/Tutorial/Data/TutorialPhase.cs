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
        public bool enabled = true;

        public List<TutorialStep> steps = new List<TutorialStep>();

        [NonSerialized]
        public TutorialPhaseEvents events = new TutorialPhaseEvents();

        /// <summary>
        /// 获取第一个启用的步骤索引
        /// </summary>
        public int GetFirstEnabledStepIndex()
        {
            for (int i = 0; i < steps.Count; i++)
            {
                if (steps[i].enabled) return i;
            }
            return -1;
        }

        /// <summary>
        /// 获取下一个启用的步骤索引
        /// </summary>
        public int GetNextEnabledStepIndex(int currentIndex)
        {
            for (int i = currentIndex + 1; i < steps.Count; i++)
            {
                if (steps[i].enabled) return i;
            }
            return -1;
        }

        /// <summary>
        /// 获取上一个启用的步骤索引
        /// </summary>
        public int GetPrevEnabledStepIndex(int currentIndex)
        {
            for (int i = currentIndex - 1; i >= 0; i--)
            {
                if (steps[i].enabled) return i;
            }
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
