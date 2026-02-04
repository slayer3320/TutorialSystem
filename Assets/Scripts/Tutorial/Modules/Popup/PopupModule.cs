using System;
using UnityEngine;
using UnityEngine.Localization;

namespace TutorialSystem
{
    /// <summary>
    /// 弹窗模块
    /// </summary>
    [Serializable]
    public class PopupModule : TutorialModuleBase
    {
        public override string ModuleName => "弹窗";

        #region Content Properties

        [SerializeField]
        [Tooltip("本地化标题（启用本地化时使用）")]
        private LocalizedString localizedTitle;

        [SerializeField]
        [Tooltip("原始标题（未启用本地化时使用）")]
        private string rawTitle;

        [SerializeField]
        [Tooltip("本地化内容（启用本地化时使用）")]
        private LocalizedString localizedContent;

        [SerializeField]
        [Tooltip("原始内容（未启用本地化时使用）")]
        [TextArea(3, 5)]
        private string rawContent;

        [SerializeField]
        [Tooltip("显示确认按钮")]
        private bool showButton = true;

        [SerializeField]
        [Tooltip("按钮本地化文本")]
        private LocalizedString buttonText;

        #endregion

        private TutorialPopupUI popupUI;

        public event Action OnButtonClicked;

        // 运行时强制使用原始文本的标记
        private bool forceRawText = false;

        protected override void OnActivate()
        {
            popupUI = TutorialUIPool.GetPopup();
            if (popupUI != null)
            {
                // 设置Canvas
                if (targetCanvas != null)
                {
                    popupUI.transform.SetParent(targetCanvas.transform, false);
                }

                bool useLocalization = !forceRawText &&
                    TutorialManager.Instance != null &&
                    TutorialManager.Instance.UseLocalization;

                string title = useLocalization && !localizedTitle.IsEmpty
                    ? localizedTitle.GetLocalizedString()
                    : rawTitle;

                string content = useLocalization && !localizedContent.IsEmpty
                    ? localizedContent.GetLocalizedString()
                    : rawContent;

                string btnText = useLocalization && !buttonText.IsEmpty
                    ? buttonText.GetLocalizedString()
                    : "OK";

                // 转换位置模式
                PopupPosition popupPosition = ConvertToPopupPosition();
                Vector2 customPos = positionMode == ModulePositionMode.Manual 
                    ? manualPosition 
                    : Vector2.zero;

                popupUI.Setup(title, content, showButton, btnText, popupPosition, 
                    customPos, positionOffset, sizeType == ModuleSizeType.CustomSize ? customSize.x : 0);
                popupUI.OnButtonClick += HandleButtonClick;

                // 初始化并播放所有配置的Effect
                var rectTransform = popupUI.GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    InitializeAndPlayEffects(rectTransform);
                }

                popupUI.Show();
            }
        }

        protected override void OnDeactivate()
        {
            if (popupUI != null)
            {
                popupUI.OnButtonClick -= HandleButtonClick;
                popupUI.Hide();
                TutorialUIPool.ReturnPopup(popupUI);
                popupUI = null;
            }
        }

        private void HandleButtonClick()
        {
            OnButtonClicked?.Invoke();
        }

        /// <summary>
        /// 将基类的PlacementType转换为PopupPosition
        /// </summary>
        private PopupPosition ConvertToPopupPosition()
        {
            if (positionMode == ModulePositionMode.Manual)
                return PopupPosition.Custom;

            return placementType switch
            {
                PlacementType.Center => PopupPosition.Center,
                PlacementType.Top => PopupPosition.Top,
                PlacementType.Bottom => PopupPosition.Bottom,
                PlacementType.Left => PopupPosition.Left,
                PlacementType.Right => PopupPosition.Right,
                PlacementType.TopLeft => PopupPosition.TopLeft,
                PlacementType.TopRight => PopupPosition.TopRight,
                PlacementType.BottomLeft => PopupPosition.BottomLeft,
                PlacementType.BottomRight => PopupPosition.BottomRight,
                _ => PopupPosition.Center
            };
        }

        // 代码动态设置内容时使用
        public void SetContent(string title, string content)
        {
            forceRawText = true;
            rawTitle = title;
            rawContent = content;
        }
    }

    /// <summary>
    /// 弹窗位置（保持向后兼容）
    /// </summary>
    public enum PopupPosition
    {
        Center,
        Top,
        Bottom,
        Left,
        Right,
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight,
        Custom
    }
}
