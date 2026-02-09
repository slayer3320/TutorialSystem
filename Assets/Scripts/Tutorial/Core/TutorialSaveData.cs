using System;
using System.Collections.Generic;

namespace TutorialSystem
{
    /// <summary>
    /// 教程存档数据
    /// 用于保存和加载教程进度
    /// </summary>
    [Serializable]
    public class TutorialSaveData
    {
        /// <summary>
        /// 已完成的教程名称列表
        /// </summary>
        public List<string> completedTutorials = new List<string>();

        /// <summary>
        /// 当前正在进行的教程名称（用于中断恢复，可选）
        /// </summary>
        public string currentTutorialName = "";

        /// <summary>
        /// 当前阶段索引
        /// </summary>
        public int currentPhaseIndex = -1;

        /// <summary>
        /// 当前步骤索引
        /// </summary>
        public int currentStepIndex = -1;
    }
}
