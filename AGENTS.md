# AGENTS.md - Tutorial System

Unity 教程引导系统 (Unity 6 / 6000.3.1f1)

## 语言偏好

- 始终使用中文进行回复和解释
- 代码注释使用中文，变量名、函数名保持英文

## 项目目标

打造一个 **Inspector 界面优美、功能完善、无 Bug、配置方便、可扩展性强** 的教程系统。

## 项目结构

```
Assets/Scripts/Tutorial/
├── Core/           # TutorialManager, TutorialRunner, TutorialBuilder
├── Data/           # TutorialConfig, TutorialPhase, TutorialStep, TutorialContext
├── Events/         # TutorialEventChannel, GameEventBus
├── Modules/        # ArrowModule, PopupModule, HighlightModule
├── Triggers/       # TimerTrigger, ButtonClickTrigger, KeyPressTrigger, etc.
├── UI/             # TutorialUIPool, TutorialArrowUI, TutorialPopupUI
└── Editor/         # 编辑器扩展（PropertyDrawer, CustomEditor, EditorWindow）
```

---

## 代码风格

### 命名空间

- 运行时: `namespace TutorialSystem`
- 编辑器: `namespace TutorialSystem.Editor`


