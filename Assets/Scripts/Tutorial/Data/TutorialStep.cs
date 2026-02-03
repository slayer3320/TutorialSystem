using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;

namespace TutorialSystem
{
    /// <summary>
    /// 教程步骤
    /// </summary>
    [Serializable]
    public class TutorialStep
    {
        public string stepId;
        public string stepName;
        public bool enabled = true;

        [Header("本地化内容")]
        public LocalizedString titleKey;
        public LocalizedString contentKey;

        [Header("完成触发器")]
        [SerializeReference]
        public ITutorialTrigger completeTrigger;

        [Header("模块配置")]
        [SerializeReference]
        public List<ITutorialModule> modules = new List<ITutorialModule>();

        [Header("事件")]
        public TutorialStepEvents events = new TutorialStepEvents();

        [Header("设置")]
        [Tooltip("进入此步骤时暂停游戏")]
        public bool pauseOnEnter = false;
        
        [Tooltip("退出时恢复游戏")]
        public bool resumeOnExit = false;
    }

    [Serializable]
    public class TutorialStepEvents
    {
        [Tooltip("进入步骤时触发")]
        public UnityEvent onEnter = new UnityEvent();
        
        [Tooltip("步骤完成时触发")]
        public UnityEvent onComplete = new UnityEvent();
        
        [Tooltip("退出步骤时触发")]
        public UnityEvent onExit = new UnityEvent();
    }
}
