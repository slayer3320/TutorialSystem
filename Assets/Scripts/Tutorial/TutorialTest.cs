using UnityEngine;

public class TutorialTest : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RectTransform buttonRect = transform.Find("Button").GetComponent<RectTransform>();
        TutorialSystem.TutorialBuilder builder = new TutorialSystem.TutorialBuilder("新手教程");
        builder
            .AddPhase("第一阶段")
            .AddStep("欢迎")
                .WithPopup("欢迎", "欢迎来到游戏！")
                .WithTimerTrigger(3f)
            .AddStep("点击按钮")
                .WithArrow(buttonRect)
                .WithHighlight(buttonRect)
            .BuildAndStart();
    }


}
