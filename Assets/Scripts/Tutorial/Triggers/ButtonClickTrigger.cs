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
        [Tooltip("目标按钮（如果为空，需在运行时设置）")]
        private Button targetButton;

        [SerializeField]
        [Tooltip("通过路径查找按钮（如果 targetButton 为空）")]
        private string buttonPath;

        private Button resolvedButton;

        public void SetTargetButton(Button button)
        {
            targetButton = button;
        }

        public override void Initialize(TutorialContext context)
        {
            base.Initialize(context);
            ResolveButton();
        }

        private void ResolveButton()
        {
            if (targetButton != null)
            {
                resolvedButton = targetButton;
                return;
            }

            if (!string.IsNullOrEmpty(buttonPath))
            {
                var go = GameObject.Find(buttonPath);
                if (go != null)
                {
                    resolvedButton = go.GetComponent<Button>();
                }
            }
        }

        public override void Enable()
        {
            base.Enable();
            if (resolvedButton != null)
            {
                resolvedButton.onClick.AddListener(OnButtonClick);
            }
        }

        public override void Disable()
        {
            base.Disable();
            if (resolvedButton != null)
            {
                resolvedButton.onClick.RemoveListener(OnButtonClick);
            }
        }

        private void OnButtonClick()
        {
            Trigger();
        }
    }
}
