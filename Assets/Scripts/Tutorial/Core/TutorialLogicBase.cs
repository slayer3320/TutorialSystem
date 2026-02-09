using System;
using UnityEngine;
using TutorialSystem;

/// <summary>
/// 教程逻辑基类
/// 提供教程归属检查、通用事件订阅/取消订阅、音效播放等公共功能
/// 所有具体的 TutorialLogic 都应该继承此类
/// </summary>
[RequireComponent(typeof(TutorialRunner))]
public abstract class TutorialLogicBase : MonoBehaviour
{
    protected TutorialRunner tutorialRunner;
    protected bool isEnabled = false;
    protected bool isRunning = false;

    #region Unity 生命周期

    protected virtual void Awake()
    {
        tutorialRunner = GetComponent<TutorialRunner>();
        if (tutorialRunner == null)
        {
            Debug.LogError($"[{GetType().Name}] 未找到 TutorialRunner 组件！");
        }
    }

    protected virtual void OnEnable()
    {
        isEnabled = true;
        SubscribeTutorialEvents();
        SubscribeGameEvents();
    }

    protected virtual void OnDisable()
    {
        isEnabled = false;
        isRunning = false;
        UnsubscribeTutorialEvents();
        UnsubscribeGameEvents();
        CleanupAllHandlers();
    }

    #endregion

    #region 事件订阅

    /// <summary>
    /// 订阅教程系统事件
    /// </summary>
    private void SubscribeTutorialEvents()
    {
        TutorialEventChannel.OnTutorialStart += HandleTutorialStart;
        TutorialEventChannel.OnTutorialComplete += HandleTutorialComplete;
        TutorialEventChannel.OnTutorialSkip += HandleTutorialSkip;
        TutorialEventChannel.OnStepEnter += HandleStepEnter;
        TutorialEventChannel.OnStepExit += HandleStepExit;
        TutorialEventChannel.OnPhaseEnter += HandlePhaseEnter;
        TutorialEventChannel.OnPhaseExit += HandlePhaseExit;
    }

    /// <summary>
    /// 取消订阅教程系统事件
    /// </summary>
    private void UnsubscribeTutorialEvents()
    {
        TutorialEventChannel.OnTutorialStart -= HandleTutorialStart;
        TutorialEventChannel.OnTutorialComplete -= HandleTutorialComplete;
        TutorialEventChannel.OnTutorialSkip -= HandleTutorialSkip;
        TutorialEventChannel.OnStepEnter -= HandleStepEnter;
        TutorialEventChannel.OnStepExit -= HandleStepExit;
        TutorialEventChannel.OnPhaseEnter -= HandlePhaseEnter;
        TutorialEventChannel.OnPhaseExit -= HandlePhaseExit;
    }

    /// <summary>
    /// 订阅游戏事件（由子类实现）
    /// </summary>
    protected virtual void SubscribeGameEvents() { }

    /// <summary>
    /// 取消订阅游戏事件（由子类实现）
    /// </summary>
    protected virtual void UnsubscribeGameEvents() { }

    #endregion

    #region 教程归属检查

    /// <summary>
    /// 检查是否是当前逻辑类对应的教程
    /// </summary>
    protected bool IsOwnTutorial(TutorialConfig config)
    {
        return tutorialRunner != null && config == tutorialRunner.Config;
    }

    /// <summary>
    /// 检查步骤是否属于当前教程
    /// </summary>
    protected bool IsOwnStep(TutorialStep step)
    {
        if (tutorialRunner == null || tutorialRunner.Config == null) return false;

        foreach (var phase in tutorialRunner.Config.phases)
        {
            if (phase.steps.Contains(step)) return true;
        }
        return false;
    }

    /// <summary>
    /// 检查阶段是否属于当前教程
    /// </summary>
    protected bool IsOwnPhase(TutorialPhase phase)
    {
        if (tutorialRunner == null || tutorialRunner.Config == null) return false;
        return tutorialRunner.Config.phases.Contains(phase);
    }

    #endregion

    #region 事件处理（带归属检查）

    private void HandleTutorialStart(TutorialConfig config)
    {
        if (!isEnabled || !IsOwnTutorial(config)) return;

        isRunning = true;
        OnTutorialStart(config);
    }

    private void HandleTutorialComplete(TutorialConfig config)
    {
        if (!isEnabled || !IsOwnTutorial(config)) return;

        isRunning = false;
        OnTutorialComplete(config);
    }

    private void HandleTutorialSkip(TutorialConfig config)
    {
        if (!isEnabled || !IsOwnTutorial(config)) return;

        isRunning = false;
        OnTutorialSkip(config);
    }

    private void HandleStepEnter(TutorialStep step)
    {
        if (!isEnabled || !isRunning || !IsOwnStep(step)) return;

        OnStepChange();
        OnStepEnter(step);
    }

    private void HandleStepExit(TutorialStep step)
    {
        if (!isEnabled || !IsOwnStep(step)) return;

        OnStepExit(step);
    }

    private void HandlePhaseEnter(TutorialPhase phase)
    {
        if (!isEnabled || !isRunning || !IsOwnPhase(phase)) return;

        OnPhaseEnter(phase);
    }

    private void HandlePhaseExit(TutorialPhase phase)
    {
        if (!isEnabled || !IsOwnPhase(phase)) return;

        OnPhaseExit(phase);
    }

    #endregion

    #region 子类可重写的事件回调

    /// <summary>
    /// 当本教程启动时调用
    /// </summary>
    protected virtual void OnTutorialStart(TutorialConfig config) { }

    /// <summary>
    /// 当本教程完成时调用
    /// </summary>
    protected virtual void OnTutorialComplete(TutorialConfig config) { }

    /// <summary>
    /// 当本教程被跳过时调用
    /// </summary>
    protected virtual void OnTutorialSkip(TutorialConfig config) { }

    /// <summary>
    /// 当进入本教程的某个步骤时调用
    /// </summary>
    protected virtual void OnStepEnter(TutorialStep step) { }

    /// <summary>
    /// 当退出本教程的某个步骤时调用
    /// </summary>
    protected virtual void OnStepExit(TutorialStep step) { }

    /// <summary>
    /// 当进入本教程的某个阶段时调用
    /// </summary>
    protected virtual void OnPhaseEnter(TutorialPhase phase) { }

    /// <summary>
    /// 当退出本教程的某个阶段时调用
    /// </summary>
    protected virtual void OnPhaseExit(TutorialPhase phase) { }

    /// <summary>
    /// 清理所有事件监听器（由子类实现）
    /// </summary>
    protected virtual void CleanupAllHandlers() { }

    #endregion

    #region 工具方法

    /// <summary>
    /// 当步骤切换时调用（无参数版，由子类实现）
    /// 可用于播放音效等通用操作
    /// </summary>
    protected virtual void OnStepChange() { }

    /// <summary>
    /// 启动教程
    /// </summary>
    public void StartTutorial()
    {
        if (tutorialRunner == null)
        {
            Debug.LogError($"[{GetType().Name}] TutorialRunner 为空，无法启动教程！");
            return;
        }

        // 检查教程是否已完成
        if (IsTutorialAlreadyCompleted())
        {
            Debug.Log($"[{GetType().Name}] 教程已完成，跳过启动");
            return;
        }

        tutorialRunner.StartTutorial();
    }

    /// <summary>
    /// 检查当前教程是否已完成（从存档中）
    /// </summary>
    protected bool IsTutorialAlreadyCompleted()
    {
        if (tutorialRunner == null || tutorialRunner.Config == null) return false;

        var progressManager = TutorialProgressManager.Instance;
        if (progressManager == null) return false;

        return progressManager.IsTutorialCompleted(tutorialRunner.Config.tutorialName);
    }

    /// <summary>
    /// 获取当前教程名称
    /// </summary>
    protected string GetTutorialName()
    {
        return tutorialRunner?.Config?.tutorialName ?? "";
    }

    /// <summary>
    /// 推进到下一步
    /// </summary>
    protected void NextStep()
    {
        TutorialManager.Instance?.NextStep();
    }

    /// <summary>
    /// 停止教程
    /// </summary>
    protected void StopTutorial()
    {
        TutorialManager.Instance?.StopTutorial();
    }

    #endregion
}
