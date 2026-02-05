using System;
using UnityEngine;
using UnityEngine.UI;

namespace TutorialSystem
{
    /// <summary>
    /// 按钮点击触发器
    /// </summary>
    [Serializable]
    public class ButtonClickTrigger : TutorialTriggerBase
    {
        public override string TriggerName => "按钮点击触发";

        [SerializeField]
        [Tooltip("目标按钮")]
        private Button targetButton;

        public void SetTargetButton(Button button)
        {
            targetButton = button;
        }

        public override void Enable()
        {
            base.Enable();
            if (targetButton != null)
            {
                targetButton.onClick.AddListener(OnButtonClick);
            }
        }

        public override void Disable()
        {
            base.Disable();
            if (targetButton != null)
            {
                targetButton.onClick.RemoveListener(OnButtonClick);
            }
        }

        private void OnButtonClick()
        {
            Trigger();
        }
    }
}
