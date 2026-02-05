using System;
using UnityEngine;

namespace TutorialSystem
{
    /// <summary>
    /// 手动触发器 - 用于通过代码逻辑手动触发教程步骤
    /// 使用场景：当某个特定游戏事件发生时，调用 TriggerManually() 方法来推进教程
    /// 例如：玩家完成某个任务、达成某个条件等
    /// </summary>
    [Serializable]
    public class ManualTrigger : TutorialTriggerBase
    {
        public override string TriggerName => "手动触发";

        /// <summary>
        /// 手动触发（可在代码中调用）
        /// </summary>
        public void TriggerManually()
        {
            Trigger();
        }
    }
}
