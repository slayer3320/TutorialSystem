using System;
using UnityEngine;

namespace TutorialSystem
{
    /// <summary>
    /// 按键触发器
    /// </summary>
    [Serializable]
    public class KeyPressTrigger : TutorialTriggerBase
    {
        public override string TriggerName => "按键触发";

        [SerializeField]
        [Tooltip("触发按键")]
        private KeyCode triggerKey = KeyCode.Space;

        [SerializeField]
        [Tooltip("是否允许任意键触发")]
        private bool anyKey = false;

        public override void Update()
        {
            if (!isEnabled || isTriggered) return;

            if (anyKey)
            {
                if (Input.anyKeyDown)
                {
                    Trigger();
                }
            }
            else
            {
                if (Input.GetKeyDown(triggerKey))
                {
                    Trigger();
                }
            }
        }
    }
}
