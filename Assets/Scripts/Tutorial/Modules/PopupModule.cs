using System;
using UnityEngine;
using UnityEngine.Localization;

namespace TutorialSystem
{
    /// <summary>
    /// 弹窗位置
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

    /// <summary>
    /// 弹窗模块
    /// </summary>
    [Serializable]
    public class PopupModule : TutorialModuleBase
    {
        public override string ModuleName => "弹窗";

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

        [SerializeField]
        private PopupPosition position = PopupPosition.Center;
        
        [SerializeField]
        [Tooltip("自定义位置（PopupPosition.Custom 时使用）")]
        private Vector2 customPosition;
        
        [SerializeField]
        [Tooltip("位置偏移")]
        private Vector2 offset;

        [SerializeField]
        [Tooltip("弹窗宽度（0 表示自动）")]
        private float width = 0f;

        private TutorialPopupUI popupUI;

        public event Action OnButtonClicked;

        // 运行时强制使用原始文本的标记
        private bool forceRawText = false;

        protected override void OnActivate()
        {
            popupUI = TutorialUIPool.GetPopup();
            if (popupUI != null)
            {
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

                popupUI.Setup(title, content, showButton, btnText, position, customPosition, offset, width);
                popupUI.OnButtonClick += HandleButtonClick;
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

        // 代码动态设置内容时使用
        public void SetContent(string title, string content)
        {
            forceRawText = true;
            rawTitle = title;
            rawContent = content;
        }
    }
}
