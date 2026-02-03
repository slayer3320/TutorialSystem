using UnityEngine;

namespace TutorialSystem.Setup
{
    /// <summary>
    /// 通过代码构建教程的演示
    /// </summary>
    public class CodeTutorialDemo : MonoBehaviour
    {
        [Header("目标 UI")]
        [SerializeField] private RectTransform targetButton;

        [Header("设置")]
        [SerializeField] private float startDelay = 0.5f;

        private void Start()
        {
            if (TutorialManager.Instance == null)
            {
                Debug.LogError("[CodeTutorialDemo] TutorialManager 未找到!");
                return;
            }

            Invoke(nameof(StartTutorial), startDelay);
        }

        private void StartTutorial()
        {
            var builder = new TutorialBuilder("代码构建教程");

            builder
                .AddPhase("欢迎阶段")
                    .AddStep("欢迎提示")
                        .WithPopup("欢迎!", "这是一个通过代码构建的教程演示。\n3秒后自动进入下一步...")
                        .WithTimerTrigger(3f);

            if (targetButton != null)
            {
                builder
                    .AddStep("点击按钮")
                        .WithPopup("操作指引", "请点击下方高亮的按钮")
                        .WithArrow(targetButton)
                        .WithHighlight(targetButton)
                        .WithButtonClickTrigger(targetButton.GetComponent<UnityEngine.UI.Button>());
            }

            builder
                .AddStep("完成")
                    .WithPopup("恭喜!", "教程已完成！\n\n你已成功体验了教程系统的基本功能。")
                    .WithTimerTrigger(3f);

            builder.BuildAndStart();
        }
    }
}
