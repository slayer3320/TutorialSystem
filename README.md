# Unity 教程系统 (Tutorial System)

一个轻量级、灵活且易于扩展的 Unity 教程系统，用于创建交互式的分步教程和新手引导体验。

## 项目简介

本项目是一个基于 Unity 的教程系统实现，采用模块化设计，支持阶段（Phase）和步骤（Step）的层级结构，适用于游戏内新手引导、功能教学等场景。

**特别说明**：本项目在开发过程中借鉴了 [HardCodeLab](https://hardcodelab.com/) 的 Tutorial Master2 插件的设计思路，但核心代码为完全重新实现。

## 核心功能

### 架构设计
- **层级结构**：Tutorial → Phase → Step 三层架构
- **模块化系统**：可插拔的模块设计（箭头、弹窗、精灵）
- **触发器系统**：支持多种触发方式（按钮点击、按键、计时器、手动）
- **事件总线**：基于事件驱动的解耦架构
- **对象池**：高效的 UI 元素复用机制

### 可视化模块
- **ArrowModule（箭头模块）**：指向目标位置的引导箭头
- **PopupModule（弹窗模块）**：显示文本说明和提示信息
- **SpriteModule（精灵模块）**：显示自定义图片和视觉元素

### 视觉效果
- **FadeInEffect（淡入效果）**：平滑的透明度渐变动画
- **FloatingEffect（浮动效果）**：上下浮动的动态效果
- **PulseEffect（脉冲效果）**：缩放脉冲动画，吸引注意力

### 触发器类型
- **ManualTrigger（手动触发）**：通过代码或 UI 按钮手动触发
- **TimerTrigger（计时触发）**：基于时间延迟自动触发
- **ButtonClickTrigger（按钮点击触发）**：点击特定 UI 按钮时触发
- **KeyPressTrigger（按键触发）**：按下指定按键时触发

### 编辑器工具
- **自定义 Inspector**：TutorialRunner 组件的可视化编辑界面
- **属性绘制器**：为各种模块提供友好的编辑器展示
- **调试窗口**：运行时调试和测试工具

## 项目结构

```
Assets/Scripts/Tutorial/
├── Core/                      # 核心管理类
│   ├── TutorialManager.cs     # 单例管理器，控制教程流程
│   └── TutorialRunner.cs      # MonoBehaviour 组件，启动教程
├── Data/                      # 数据结构
│   ├── TutorialConfig.cs      # 教程配置（包含多个 Phase）
│   ├── TutorialPhase.cs       # 阶段配置（包含多个 Step）
│   ├── TutorialStep.cs        # 步骤配置（包含模块和触发器）
│   └── TutorialContext.cs     # 运行时上下文
├── Modules/                   # 可视化模块
│   ├── ITutorialModule.cs     # 模块接口
│   ├── TutorialModuleBase.cs  # 模块基类
│   ├── ArrowModule.cs         # 箭头模块
│   ├── PopupModule.cs         # 弹窗模块
│   └── SpriteModule.cs        # 精灵模块
├── Triggers/                  # 触发器系统
│   ├── ITutorialTrigger.cs    # 触发器接口
│   ├── TutorialTriggerBase.cs # 触发器基类
│   ├── ManualTrigger.cs       # 手动触发
│   ├── TimerTrigger.cs        # 计时触发
│   ├── ButtonClickTrigger.cs  # 按钮点击触发
│   └── KeyPressTrigger.cs     # 按键触发
├── Effects/                   # 视觉效果
│   ├── IEffect.cs             # 效果接口
│   ├── EffectBase.cs          # 效果基类
│   ├── EffectSettings.cs      # 效果配置
│   ├── FadeInEffect.cs        # 淡入效果
│   ├── FloatingEffect.cs      # 浮动效果
│   └── PulseEffect.cs         # 脉冲效果
├── Events/                    # 事件系统
│   ├── GameEventBus.cs        # 通用事件总线
│   └── TutorialEventChannel.cs # 教程事件频道
├── UI/                        # UI 组件
│   ├── TutorialUIPool.cs      # UI 对象池
│   ├── TutorialArrowUI.cs     # 箭头 UI
│   ├── TutorialPopupUI.cs     # 弹窗 UI
│   └── TutorialSpriteUI.cs    # 精灵 UI
└── Editor/                    # 编辑器扩展
    ├── CustomEditors/         # 自定义编辑器
    ├── PropertyDrawers/       # 属性绘制器
    └── Windows/               # 编辑器窗口
```

## 快速开始

### 系统要求
- Unity 2020.3 或更高版本
- TextMesh Pro

### 基本使用步骤

#### 1. 场景设置
1. 在场景中创建一个空 GameObject
2. 添加 `TutorialRunner` 组件
3. 在 Inspector 中配置教程内容

#### 2. 配置教程
在 TutorialRunner 的 Inspector 中：
- 设置教程名称
- 添加 Phase（阶段）
- 为每个 Phase 添加 Step（步骤）
- 为每个 Step 配置模块和触发器

#### 3. 代码示例

```csharp
using TutorialSystem;

// 启动教程
TutorialConfig config = ...; // 从 ScriptableObject 或其他地方获取
TutorialManager.Instance.StartTutorial(config);

// 手动控制流程
TutorialManager.Instance.NextStep();     // 下一步
TutorialManager.Instance.PrevStep();     // 上一步
TutorialManager.Instance.NextPhase();    // 下一阶段
TutorialManager.Instance.SkipTutorial(); // 跳过教程
TutorialManager.Instance.StopTutorial(); // 停止教程

// 跳转到指定位置
TutorialManager.Instance.JumpToPhase(1);      // 跳转到第 2 个阶段
TutorialManager.Instance.JumpToStep(1, 2);    // 跳转到第 2 阶段的第 3 步

// 监听事件
TutorialEventChannel.OnTutorialStart += OnTutorialStarted;
TutorialEventChannel.OnStepEnter += OnStepEnter;
TutorialEventChannel.OnTutorialComplete += OnTutorialCompleted;
```

#### 4. 自定义模块示例

```csharp
using TutorialSystem;
using UnityEngine;

public class CustomModule : TutorialModuleBase
{
    public override void Activate()
    {
        base.Activate();
        // 激活时的逻辑
    }

    public override void Deactivate()
    {
        base.Deactivate();
        // 停用时的逻辑
    }

    public override void UpdateModule()
    {
        // 每帧更新的逻辑
    }
}
```

## 核心概念

### Tutorial（教程）
- 顶层容器，包含多个 Phase
- 控制整个教程的生命周期
- 支持跳过设置

### Phase（阶段）
- 教程的逻辑分组单位
- 包含多个 Step
- 可独立触发进入/退出事件

### Step（步骤）
- 最小的教程单元
- 包含可视化模块（箭头、弹窗等）
- 配置触发器来控制完成条件
- 支持暂停/恢复游戏时间

### Module（模块）
- 可视化元素的抽象
- 支持效果系统
- 通过接口实现高度可扩展

### Trigger（触发器）
- 决定步骤何时完成
- 支持多种触发方式
- 可自定义扩展

## 开发特性

- ✅ 完全基于 Unity 序列化系统
- ✅ 支持运行时动态创建教程
- ✅ 事件驱动架构，易于集成
- ✅ 对象池优化性能
- ✅ 编辑器友好的可视化配置
- ✅ 中文注释和文档

## 扩展性

系统采用接口和抽象类设计，方便扩展：

1. **自定义模块**：实现 `ITutorialModule` 接口或继承 `TutorialModuleBase`
2. **自定义触发器**：实现 `ITutorialTrigger` 接口或继承 `TutorialTriggerBase`
3. **自定义效果**：实现 `IEffect` 接口或继承 `EffectBase`
4. **事件监听**：通过 `TutorialEventChannel` 订阅各类事件

## 最近更新

根据 Git 提交历史：
- ✅ Trigger 系统重组和优化
- ✅ 移除 Highlight 模块，新增 Sprite 模块
- ✅ 资源整理和代码精简
- ✅ 移除 Setup 组件

## 致谢

- 本项目在设计上参考了 [HardCodeLab](https://hardcodelab.com/) 的 **Tutorial Master2** 插件

## 许可证

本项目代码（`Assets/Scripts/Tutorial/` 目录）可自由使用和修改。

---

**开发环境**：Unity 2020.3+ | C# | URP
