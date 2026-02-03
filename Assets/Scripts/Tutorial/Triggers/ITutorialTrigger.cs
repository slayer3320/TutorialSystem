using System;

namespace TutorialSystem
{
    /// <summary>
    /// 教程触发器接口
    /// </summary>
    public interface ITutorialTrigger
    {
        /// <summary>
        /// 触发器名称（用于编辑器显示）
        /// </summary>
        string TriggerName { get; }
        
        /// <summary>
        /// 是否已触发
        /// </summary>
        bool IsTriggered { get; }
        
        /// <summary>
        /// 是否启用
        /// </summary>
        bool IsEnabled { get; }

        /// <summary>
        /// 初始化触发器
        /// </summary>
        void Initialize(TutorialContext context);
        
        /// <summary>
        /// 启用触发器（开始监听）
        /// </summary>
        void Enable();
        
        /// <summary>
        /// 禁用触发器（停止监听）
        /// </summary>
        void Disable();
        
        /// <summary>
        /// 重置触发器状态
        /// </summary>
        void Reset();
        
        /// <summary>
        /// 每帧更新（用于轮询类触发器）
        /// </summary>
        void Update();

        /// <summary>
        /// 触发时的事件
        /// </summary>
        event Action OnTriggered;
    }
}
