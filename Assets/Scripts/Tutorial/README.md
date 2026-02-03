# Tutorial System

教程引导系统，支持多阶段、多步骤的教程流程控制。

## 架构

```
TutorialConfig (配置)
    └── List<TutorialPhase> (阶段)
            └── List<TutorialStep> (步骤)
                    ├── ITutorialTrigger (完成触发器)
                    └── List<ITutorialModule> (模块)
```

## 核心组件

| 组件 | 职责 |
|------|------|
| `TutorialManager` | 单例，控制教程流程 |
| `TutorialRunner` | 场景入口，持有配置 |
| `TutorialBuilder` | 代码动态构建教程 |
| `TutorialContext` | 运行时状态 |

## 快速开始

### 1. 场景配置

1. 创建空物体，添加 `TutorialManager` 组件
2. 设置 Arrow/Popup 预制体和 UI 容器
3. 创建另一个物体，添加 `TutorialRunner` 组件
4. 在 Inspector 中配置教程阶段和步骤

### 2. 代码构建

```csharp
TutorialRunner.CreateBuilder("新手教程")
    .AddPhase("第一阶段")
    .AddStep("欢迎")
        .WithPopup("欢迎", "欢迎来到游戏！")
        .WithTimerTrigger(3f)
    .AddStep("点击按钮")
        .WithArrow(buttonRect)
        .WithHighlight(buttonRect)
    .BuildAndStart();
```

## 模块 (Modules)

模块负责显示教程 UI 元素。

| 模块 | 说明 |
|------|------|
| `PopupModule` | 弹窗提示 |
| `ArrowModule` | 指向箭头 |
| `HighlightModule` | 高亮遮罩 |

### 自定义模块

```csharp
[Serializable]
public class MyModule : TutorialModuleBase
{
    public override string ModuleName => "自定义模块";
    
    protected override void OnActivate() { }
    protected override void OnDeactivate() { }
}
```

## 触发器 (Triggers)

触发器决定何时完成当前步骤。

| 触发器 | 说明 |
|--------|------|
| `TimerTrigger` | 延时触发 |
| `ButtonClickTrigger` | 按钮点击 |
| `KeyPressTrigger` | 按键 |
| `GameEventTrigger` | 游戏事件 |
| `ManualTrigger` | 手动调用 |

### 游戏事件触发

```csharp
// 触发教程事件
GameEventBus.Publish("player_level_up");
```

## 事件

### 生命周期事件

```csharp
TutorialEventChannel.OnTutorialStart += config => { };
TutorialEventChannel.OnPhaseEnter += phase => { };
TutorialEventChannel.OnStepEnter += step => { };
TutorialEventChannel.OnStepComplete += step => { };
TutorialEventChannel.OnTutorialComplete += config => { };
```

### TutorialRunner 事件

```csharp
runner.OnTutorialStarted += () => { };
runner.OnPhaseChanged += phase => { };
runner.OnStepChanged += step => { };
runner.OnTutorialCompleted += () => { };
```

## API

### TutorialManager

```csharp
TutorialManager.Instance.StartTutorial(config);
TutorialManager.Instance.StopTutorial();
TutorialManager.Instance.SkipTutorial();
TutorialManager.Instance.NextStep();
TutorialManager.Instance.PrevStep();
TutorialManager.Instance.JumpToPhase(index);
TutorialManager.Instance.JumpToStep(phaseIndex, stepIndex);

// 进度查询
bool completed = TutorialManager.IsTutorialCompleted("tutorial_id");
TutorialManager.ResetTutorialProgress("tutorial_id");
```

### TutorialRunner

```csharp
runner.StartTutorial();
runner.StartTutorialDelayed(2f);
runner.StartTutorialWhen(() => player.Level >= 5);
```

## 文件结构

```
Tutorial/
├── Core/
│   ├── TutorialManager.cs
│   ├── TutorialRunner.cs
│   └── TutorialBuilder.cs
├── Data/
│   ├── TutorialConfig.cs
│   ├── TutorialPhase.cs
│   ├── TutorialStep.cs
│   └── TutorialContext.cs
├── Events/
│   ├── TutorialEventChannel.cs
│   └── GameEventBus.cs
├── Modules/
│   ├── ITutorialModule.cs
│   ├── TutorialModuleBase.cs
│   ├── ArrowModule.cs
│   ├── PopupModule.cs
│   └── HighlightModule.cs
├── Triggers/
│   ├── ITutorialTrigger.cs
│   ├── TutorialTriggerBase.cs
│   ├── ButtonClickTrigger.cs
│   ├── GameEventTrigger.cs
│   ├── KeyPressTrigger.cs
│   ├── ManualTrigger.cs
│   └── TimerTrigger.cs
└── UI/
    ├── TutorialUIPool.cs
    ├── TutorialArrowUI.cs
    ├── TutorialPopupUI.cs
    └── TutorialHighlightUI.cs
```
