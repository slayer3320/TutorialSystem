using System;
using UnityEngine;

namespace TutorialSystem
{
    /// <summary>
    /// 教程模块接口
    /// </summary>
    public interface ITutorialModule
    {
        /// <summary>
        /// 模块名称（用于编辑器显示）
        /// </summary>
        string ModuleName { get; }
        
        /// <summary>
        /// 是否处于激活状态
        /// </summary>
        bool IsActive { get; }

        /// <summary>
        /// 初始化模块
        /// </summary>
        void Initialize(TutorialContext context);
        
        /// <summary>
        /// 激活模块
        /// </summary>
        void Activate();
        
        /// <summary>
        /// 停用模块
        /// </summary>
        void Deactivate();
        
        /// <summary>
        /// 每帧更新
        /// </summary>
        void UpdateModule();
    }
}
