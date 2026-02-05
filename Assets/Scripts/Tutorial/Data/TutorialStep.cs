using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TutorialSystem
{
    /// <summary>
    /// 教程步骤
    /// </summary>
    [Serializable]
    public class TutorialStep
    {
        public string stepName;

        [SerializeReference]
        public ITutorialTrigger completeTrigger;


        [SerializeReference]
        public List<ITutorialModule> modules = new List<ITutorialModule>();


        [NonSerialized]
        public TutorialStepEvents events = new TutorialStepEvents();


        [Tooltip("Pause game when entering this step")]
        public bool pauseOnEnter = false;
        
        [Tooltip("Resume game on exit")]
        public bool resumeOnExit = false;
    }

    public class TutorialStepEvents
    {
        [Tooltip("Triggered when entering step")]
        public UnityEvent onEnter = new UnityEvent();
        
        [Tooltip("Triggered when step complete")]
        public UnityEvent onComplete = new UnityEvent();
        
        [Tooltip("Triggered when exiting step")]
        public UnityEvent onExit = new UnityEvent();
    }
}
