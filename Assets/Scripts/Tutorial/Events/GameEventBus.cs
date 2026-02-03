using System;
using System.Collections.Generic;

namespace TutorialSystem
{
    /// <summary>
    /// 游戏事件总线 - 用于教程系统监听游戏内事件
    /// </summary>
    public static class GameEventBus
    {
        private static readonly Dictionary<string, Action> events = new Dictionary<string, Action>();

        /// <summary>
        /// 添加事件监听
        /// </summary>
        public static void Subscribe(string eventName, Action callback)
        {
            if (string.IsNullOrEmpty(eventName) || callback == null) return;

            if (events.TryGetValue(eventName, out var existing))
                events[eventName] = existing + callback;
            else
                events[eventName] = callback;
        }

        /// <summary>
        /// 移除事件监听
        /// </summary>
        public static void Unsubscribe(string eventName, Action callback)
        {
            if (string.IsNullOrEmpty(eventName) || callback == null) return;

            if (events.TryGetValue(eventName, out var existing))
            {
                existing -= callback;
                if (existing == null)
                    events.Remove(eventName);
                else
                    events[eventName] = existing;
            }
        }

        /// <summary>
        /// 触发事件
        /// </summary>
        public static void Publish(string eventName)
        {
            if (string.IsNullOrEmpty(eventName)) return;
            if (events.TryGetValue(eventName, out var callback))
                callback?.Invoke();
        }

        /// <summary>
        /// 清除所有事件
        /// </summary>
        public static void Clear() => events.Clear();
    }
}
