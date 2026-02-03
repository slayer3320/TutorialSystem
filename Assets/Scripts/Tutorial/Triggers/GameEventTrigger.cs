using System;
using UnityEngine;

namespace TutorialSystem
{
    /// <summary>
    /// 游戏事件触发器 - 使用教程系统内部事件通道
    /// </summary>
    [Serializable]
    public class GameEventTrigger : TutorialTriggerBase
    {
        public override string TriggerName => "游戏事件触发";

        [SerializeField]
        [Tooltip("监听的事件名称")]
        private string eventName;

        public string EventName => eventName;

        private Action eventHandler;

        public override void Initialize(TutorialContext context)
        {
            base.Initialize(context);
            eventHandler = OnEventTriggered;
        }

        public override void Enable()
        {
            base.Enable();
            if (!string.IsNullOrEmpty(eventName))
            {
                GameEventBus.Subscribe(eventName, eventHandler);
            }
        }

        public override void Disable()
        {
            base.Disable();
            if (!string.IsNullOrEmpty(eventName))
            {
                GameEventBus.Unsubscribe(eventName, eventHandler);
            }
        }

        private void OnEventTriggered()
        {
            Trigger();
        }
    }
}
