using System;
using UnityEngine;

namespace TutorialSystem
{
    /// <summary>
    /// 手动触发器 - 需要代码调用 Trigger() 方法
    /// </summary>
    [Serializable]
    public class ManualTrigger : TutorialTriggerBase
    {
        public override string TriggerName => "手动触发";

        /// <summary>
        /// 手动触发
        /// </summary>
        public void TriggerManually()
        {
            Trigger();
        }
    }
}
