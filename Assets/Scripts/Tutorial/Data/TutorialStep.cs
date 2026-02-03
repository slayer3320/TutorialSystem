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


        [Tooltip("Localized Title (Used when localization is enabled)")]
        public LocalizedString localizedTitle;
        
        [Tooltip("Raw Title (Used when localization is disabled)")]
        public string rawTitle;
        
        [Tooltip("Localized Content (Used when localization is enabled)")]
        public LocalizedString localizedContent;
        
        [Tooltip("Raw Content (Used when localization is disabled)")]
        [TextArea(2, 4)]
        public string rawContent;


        [SerializeReference]
        public ITutorialTrigger completeTrigger;


        [SerializeReference]
        public List<ITutorialModule> modules = new List<ITutorialModule>();


        public TutorialStepEvents events = new TutorialStepEvents();


        [Tooltip("Pause game when entering this step")]
        public bool pauseOnEnter = false;
        
        [Tooltip("Resume game on exit")]
        public bool resumeOnExit = false;

        // 获取标题（根据全局本地化设置）
        public string GetTitle()
        {
            bool useLocalization = TutorialManager.Instance != null && 
                TutorialManager.Instance.UseLocalization;
            
            if (useLocalization && !localizedTitle.IsEmpty)
                return localizedTitle.GetLocalizedString();
            return rawTitle;
        }

        // 获取内容（根据全局本地化设置）
        public string GetContent()
        {
            bool useLocalization = TutorialManager.Instance != null && 
                TutorialManager.Instance.UseLocalization;
            
            if (useLocalization && !localizedContent.IsEmpty)
                return localizedContent.GetLocalizedString();
            return rawContent;
        }
    }

    [Serializable]
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
