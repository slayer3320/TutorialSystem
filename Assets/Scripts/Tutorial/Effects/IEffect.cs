using UnityEngine;

namespace TutorialSystem
{
    /// <summary>
    /// Effect接口
    /// </summary>
    public interface IEffect
    {
        /// <summary>
        /// 是否启用
        /// </summary>
        bool Enabled { get; set; }

        /// <summary>
        /// 是否正在播放
        /// </summary>
        bool IsPlaying { get; }

        /// <summary>
        /// 初始化Effect
        /// </summary>
        void Initialize(RectTransform target);

        /// <summary>
        /// 开始播放
        /// </summary>
        void Play();

        /// <summary>
        /// 停止播放
        /// </summary>
        void Stop();

        /// <summary>
        /// 每帧更新
        /// </summary>
        void Update();

        /// <summary>
        /// 重置到初始状态
        /// </summary>
        void Reset();
    }
}
