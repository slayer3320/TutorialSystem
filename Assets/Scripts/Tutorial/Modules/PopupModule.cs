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

        [Header("内容设置")]
        [SerializeField]
        [Tooltip("使用本地化标题")]
        private bool useLocalizedTitle = true;
        
        [SerializeField]
        [Tooltip("本地化标题")]
        private LocalizedString localizedTitle;
        
        [SerializeField]
        [Tooltip("非本地化标题（当 useLocalizedTitle 为 false 时使用）")]
        private string rawTitle;
        
        [SerializeField]
        [Tooltip("使用本地化内容")]
        private bool useLocalizedContent = true;
        
        [SerializeField]
        [Tooltip("本地化内容")]
        private LocalizedString localizedContent;
        
        [SerializeField]
        [Tooltip("非本地化内容（当 useLocalizedContent 为 false 时使用）")]
        [TextArea(3, 5)]
        private string rawContent;

        [Header("按钮设置")]
        [SerializeField]
        [Tooltip("显示确认按钮")]
        private bool showButton = true;
        
        [SerializeField]
        [Tooltip("按钮本地化文本")]
        private LocalizedString buttonText;

        [Header("位置设置")]
        [SerializeField]
        private PopupPosition position = PopupPosition.Center;
        
        [SerializeField]
        [Tooltip("自定义位置（PopupPosition.Custom 时使用）")]
        private Vector2 customPosition;
        
        [SerializeField]
        [Tooltip("位置偏移")]
        private Vector2 offset;

        [Header("外观设置")]
        [SerializeField]
        [Tooltip("弹窗宽度（0 表示自动）")]
        private float width = 0f;

        private TutorialPopupUI popupUI;

        public event Action OnButtonClicked;

        protected override void OnActivate()
        {
            popupUI = TutorialUIPool.GetPopup();
            if (popupUI != null)
            {
                string title = useLocalizedTitle && !localizedTitle.IsEmpty 
                    ? localizedTitle.GetLocalizedString() 
                    : rawTitle;
                    
                string content = useLocalizedContent && !localizedContent.IsEmpty 
                    ? localizedContent.GetLocalizedString() 
                    : rawContent;
                
                string btnText = !buttonText.IsEmpty ? buttonText.GetLocalizedString() : "OK";

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

        public void SetContent(string title, string content)
        {
            useLocalizedTitle = false;
            useLocalizedContent = false;
            rawTitle = title;
            rawContent = content;
        }
    }
}
