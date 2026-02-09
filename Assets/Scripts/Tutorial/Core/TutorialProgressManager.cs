using System;
using System.Collections.Generic;
using UnityEngine;

namespace TutorialSystem
{
    /// <summary>
    /// 教程进度管理器
    /// 负责记录教程完成状态，与存档系统同步
    /// </summary>
    public class TutorialProgressManager : MonoBehaviour
    {
        public static TutorialProgressManager Instance { get; private set; }

        [Header("调试")]
        [SerializeField] private bool debugMode = false;

        /// <summary>
        /// 已完成的教程名称集合
        /// </summary>
        private HashSet<string> completedTutorials = new HashSet<string>();

        /// <summary>
        /// 当教程完成时触发
        /// </summary>
        public static event Action<string> OnTutorialCompleted;

        #region Unity 生命周期

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void OnEnable()
        {
            TutorialEventChannel.OnTutorialComplete += HandleTutorialComplete;
            TutorialEventChannel.OnTutorialSkip += HandleTutorialSkip;
        }

        private void OnDisable()
        {
            TutorialEventChannel.OnTutorialComplete -= HandleTutorialComplete;
            TutorialEventChannel.OnTutorialSkip -= HandleTutorialSkip;
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        #endregion

        #region 事件处理

        private void HandleTutorialComplete(TutorialConfig config)
        {
            if (config == null || string.IsNullOrEmpty(config.tutorialName)) return;

            MarkTutorialCompleted(config.tutorialName);
        }

        private void HandleTutorialSkip(TutorialConfig config)
        {
            if (config == null || string.IsNullOrEmpty(config.tutorialName)) return;

            // 跳过的教程也视为完成
            MarkTutorialCompleted(config.tutorialName);
        }

        #endregion

        #region 公共 API

        /// <summary>
        /// 标记教程为已完成
        /// </summary>
        public void MarkTutorialCompleted(string tutorialName)
        {
            if (string.IsNullOrEmpty(tutorialName)) return;

            if (completedTutorials.Add(tutorialName))
            {
                Log($"教程完成: {tutorialName}");
                OnTutorialCompleted?.Invoke(tutorialName);
            }
        }

        /// <summary>
        /// 检查教程是否已完成
        /// </summary>
        public bool IsTutorialCompleted(string tutorialName)
        {
            if (string.IsNullOrEmpty(tutorialName)) return false;
            return completedTutorials.Contains(tutorialName);
        }

        /// <summary>
        /// 获取所有已完成的教程列表
        /// </summary>
        public List<string> GetCompletedTutorials()
        {
            return new List<string>(completedTutorials);
        }

        /// <summary>
        /// 设置已完成的教程列表（从存档加载时调用）
        /// </summary>
        public void SetCompletedTutorials(List<string> tutorials)
        {
            completedTutorials.Clear();
            if (tutorials != null)
            {
                foreach (var t in tutorials)
                {
                    if (!string.IsNullOrEmpty(t))
                    {
                        completedTutorials.Add(t);
                    }
                }
            }
            Log($"从存档恢复教程进度，已完成 {completedTutorials.Count} 个教程");
        }

        /// <summary>
        /// 清除所有教程进度（用于新游戏）
        /// </summary>
        public void ClearAllProgress()
        {
            completedTutorials.Clear();
            Log("清除所有教程进度");
        }

        /// <summary>
        /// 导出存档数据
        /// </summary>
        public TutorialSaveData ExportSaveData()
        {
            var data = new TutorialSaveData
            {
                completedTutorials = GetCompletedTutorials()
            };

            // 如果有正在运行的教程，记录当前进度
            if (TutorialManager.Instance != null && TutorialManager.Instance.IsRunning)
            {
                var config = TutorialManager.Instance.CurrentConfig;
                if (config != null)
                {
                    data.currentTutorialName = config.tutorialName;
                    data.currentPhaseIndex = TutorialManager.Instance.CurrentPhaseIndex;
                    data.currentStepIndex = TutorialManager.Instance.CurrentStepIndex;
                }
            }

            return data;
        }

        /// <summary>
        /// 从存档数据导入
        /// </summary>
        public void ImportSaveData(TutorialSaveData data)
        {
            if (data == null) return;

            SetCompletedTutorials(data.completedTutorials);

            // 注意：中断恢复功能需要额外的 TutorialRunner 查找逻辑
            // 这里暂不实现自动恢复，因为需要知道如何找到对应的 TutorialRunner
            if (!string.IsNullOrEmpty(data.currentTutorialName))
            {
                Log($"检测到未完成的教程: {data.currentTutorialName} (Phase: {data.currentPhaseIndex}, Step: {data.currentStepIndex})");
                // 可以通过事件通知对应的 TutorialLogic 来恢复
            }
        }

        #endregion

        #region 工具方法

        private void Log(string message)
        {
            if (debugMode)
            {
                Debug.Log($"[TutorialProgressManager] {message}");
            }
        }

        #endregion
    }
}
