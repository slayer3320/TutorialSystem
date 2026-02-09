using System;
using System.Collections.Generic;
using UnityEngine;

namespace TutorialSystem
{
    /// <summary>
    /// 教程序列控制器
    /// 统一管理教程的顺序和启动，确保按顺序执行并跳过已完成的教程
    /// </summary>
    public class TutorialSequenceController : MonoBehaviour
    {
        public static TutorialSequenceController Instance { get; private set; }

        [Header("教程序列配置")]
        [Tooltip("按顺序排列的教程逻辑，索引越小优先级越高")]
        [SerializeField] private List<TutorialLogicBase> tutorialSequence = new List<TutorialLogicBase>();

        [Header("调试")]
        [SerializeField] private bool debugMode = false;

        /// <summary>
        /// 当前教程索引（-1 表示未开始或全部完成）
        /// </summary>
        private int currentTutorialIndex = -1;

        /// <summary>
        /// 当前活跃的教程逻辑
        /// </summary>
        public TutorialLogicBase CurrentTutorial =>
            currentTutorialIndex >= 0 && currentTutorialIndex < tutorialSequence.Count
                ? tutorialSequence[currentTutorialIndex]
                : null;

        /// <summary>
        /// 当前教程索引
        /// </summary>
        public int CurrentTutorialIndex => currentTutorialIndex;

        /// <summary>
        /// 教程总数
        /// </summary>
        public int TotalTutorialCount => tutorialSequence.Count;

        /// <summary>
        /// 是否所有教程都已完成
        /// </summary>
        public bool AllTutorialsCompleted => GetNextUncompletedIndex(0) < 0;

        /// <summary>
        /// 当教程序列全部完成时触发
        /// </summary>
        public static event Action OnAllTutorialsCompleted;

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
            if (config == null) return;

            // 检查是否是当前序列中的教程完成了
            int completedIndex = FindTutorialIndex(config.tutorialName);
            if (completedIndex >= 0)
            {
                Log($"教程完成: {config.tutorialName} (索引: {completedIndex})");

                // 如果完成的是当前教程，重置索引
                if (completedIndex == currentTutorialIndex)
                {
                    currentTutorialIndex = -1;
                }

                // 检查是否全部完成
                if (AllTutorialsCompleted)
                {
                    Log("所有教程已完成！");
                    OnAllTutorialsCompleted?.Invoke();
                }
            }
        }

        private void HandleTutorialSkip(TutorialConfig config)
        {
            // 跳过也视为完成
            HandleTutorialComplete(config);
        }

        #endregion

        #region 公共 API

        /// <summary>
        /// 尝试启动下一个未完成的教程
        /// </summary>
        /// <returns>是否成功启动了教程</returns>
        public bool TryStartNextTutorial()
        {
            int nextIndex = GetNextUncompletedIndex(0);
            if (nextIndex < 0)
            {
                Log("没有未完成的教程");
                return false;
            }

            return TryStartTutorialAt(nextIndex);
        }

        /// <summary>
        /// 尝试启动指定索引的教程
        /// </summary>
        public bool TryStartTutorialAt(int index)
        {
            if (index < 0 || index >= tutorialSequence.Count)
            {
                Log($"无效的教程索引: {index}");
                return false;
            }

            var tutorial = tutorialSequence[index];
            if (tutorial == null)
            {
                Log($"索引 {index} 处的教程为空");
                return false;
            }

            // 检查是否已完成
            if (IsTutorialCompleted(tutorial))
            {
                Log($"教程已完成，跳过: {GetTutorialName(tutorial)}");
                return false;
            }

            // 启动教程
            currentTutorialIndex = index;
            Log($"启动教程: {GetTutorialName(tutorial)} (索引: {index})");
            tutorial.StartTutorial();
            return true;
        }

        /// <summary>
        /// 尝试按名称启动教程
        /// </summary>
        public bool TryStartTutorialByName(string tutorialName)
        {
            int index = FindTutorialIndex(tutorialName);
            if (index < 0)
            {
                Log($"未找到教程: {tutorialName}");
                return false;
            }
            return TryStartTutorialAt(index);
        }

        /// <summary>
        /// 获取下一个未完成的教程索引
        /// </summary>
        /// <param name="startFrom">从哪个索引开始查找</param>
        /// <returns>未完成教程的索引，-1 表示全部完成</returns>
        public int GetNextUncompletedIndex(int startFrom = 0)
        {
            for (int i = startFrom; i < tutorialSequence.Count; i++)
            {
                var tutorial = tutorialSequence[i];
                if (tutorial != null && !IsTutorialCompleted(tutorial))
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// 获取指定教程是否已完成
        /// </summary>
        public bool IsTutorialCompleted(TutorialLogicBase tutorial)
        {
            if (tutorial == null) return true;

            var runner = tutorial.GetComponent<TutorialRunner>();
            if (runner == null || runner.Config == null) return true;

            var progressManager = TutorialProgressManager.Instance;
            if (progressManager == null) return false;

            return progressManager.IsTutorialCompleted(runner.Config.tutorialName);
        }

        /// <summary>
        /// 获取教程名称
        /// </summary>
        public string GetTutorialName(TutorialLogicBase tutorial)
        {
            if (tutorial == null) return "";
            var runner = tutorial.GetComponent<TutorialRunner>();
            return runner?.Config?.tutorialName ?? tutorial.GetType().Name;
        }

        /// <summary>
        /// 获取教程列表信息（用于调试）
        /// </summary>
        public List<TutorialSequenceInfo> GetSequenceInfo()
        {
            var result = new List<TutorialSequenceInfo>();
            for (int i = 0; i < tutorialSequence.Count; i++)
            {
                var tutorial = tutorialSequence[i];
                result.Add(new TutorialSequenceInfo
                {
                    index = i,
                    name = GetTutorialName(tutorial),
                    isCompleted = tutorial != null && IsTutorialCompleted(tutorial),
                    isCurrent = i == currentTutorialIndex,
                    logic = tutorial
                });
            }
            return result;
        }

        #endregion

        #region 内部方法

        private int FindTutorialIndex(string tutorialName)
        {
            for (int i = 0; i < tutorialSequence.Count; i++)
            {
                var tutorial = tutorialSequence[i];
                if (tutorial != null && GetTutorialName(tutorial) == tutorialName)
                {
                    return i;
                }
            }
            return -1;
        }

        private void Log(string message)
        {
            if (debugMode)
            {
                Debug.Log($"[TutorialSequenceController] {message}");
            }
        }

        #endregion
    }

    /// <summary>
    /// 教程序列信息（用于调试和UI显示）
    /// </summary>
    [Serializable]
    public class TutorialSequenceInfo
    {
        public int index;
        public string name;
        public bool isCompleted;
        public bool isCurrent;
        public TutorialLogicBase logic;
    }
}
