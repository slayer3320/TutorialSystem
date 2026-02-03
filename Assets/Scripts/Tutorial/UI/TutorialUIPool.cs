using System.Collections.Generic;
using UnityEngine;

namespace TutorialSystem
{
    /// <summary>
    /// 教程 UI 对象池
    /// </summary>
    public static class TutorialUIPool
    {
        private static Queue<TutorialArrowUI> arrowPool = new Queue<TutorialArrowUI>();
        private static Queue<TutorialPopupUI> popupPool = new Queue<TutorialPopupUI>();
        private static Queue<TutorialHighlightUI> highlightPool = new Queue<TutorialHighlightUI>();

        private static GameObject arrowPrefab;
        private static GameObject popupPrefab;
        private static GameObject highlightPrefab;
        private static Transform poolContainer;

        public static void Initialize(GameObject arrowPrefab, GameObject popupPrefab, Transform container)
        {
            TutorialUIPool.arrowPrefab = arrowPrefab;
            TutorialUIPool.popupPrefab = popupPrefab;
            poolContainer = container;
        }

        public static void SetHighlightPrefab(GameObject prefab)
        {
            highlightPrefab = prefab;
        }

        public static void PrewarmPool(int arrowCount = 2, int popupCount = 1, int highlightCount = 1)
        {
            for (int i = 0; i < arrowCount; i++)
            {
                var arrow = CreateArrow();
                if (arrow != null)
                {
                    arrow.Hide();
                    arrowPool.Enqueue(arrow);
                }
            }

            for (int i = 0; i < popupCount; i++)
            {
                var popup = CreatePopup();
                if (popup != null)
                {
                    popup.Hide();
                    popupPool.Enqueue(popup);
                }
            }

            for (int i = 0; i < highlightCount; i++)
            {
                var highlight = CreateHighlight();
                if (highlight != null)
                {
                    highlight.Hide();
                    highlightPool.Enqueue(highlight);
                }
            }
        }

        #region Arrow

        public static TutorialArrowUI GetArrow()
        {
            if (arrowPool.Count > 0)
                return arrowPool.Dequeue();
            return CreateArrow();
        }

        public static void ReturnArrow(TutorialArrowUI arrow)
        {
            if (arrow != null)
            {
                arrow.Hide();
                arrowPool.Enqueue(arrow);
            }
        }

        private static TutorialArrowUI CreateArrow()
        {
            if (arrowPrefab == null)
            {
                Debug.LogError("[TutorialUIPool] Arrow prefab is not set!");
                return null;
            }
            var go = Object.Instantiate(arrowPrefab, poolContainer);
            return go.GetComponent<TutorialArrowUI>();
        }

        #endregion

        #region Popup

        public static TutorialPopupUI GetPopup()
        {
            if (popupPool.Count > 0)
                return popupPool.Dequeue();
            return CreatePopup();
        }

        public static void ReturnPopup(TutorialPopupUI popup)
        {
            if (popup != null)
            {
                popup.Hide();
                popupPool.Enqueue(popup);
            }
        }

        private static TutorialPopupUI CreatePopup()
        {
            if (popupPrefab == null)
            {
                Debug.LogError("[TutorialUIPool] Popup prefab is not set!");
                return null;
            }
            var go = Object.Instantiate(popupPrefab, poolContainer);
            return go.GetComponent<TutorialPopupUI>();
        }

        #endregion

        #region Highlight

        public static TutorialHighlightUI GetHighlight()
        {
            if (highlightPool.Count > 0)
                return highlightPool.Dequeue();
            return CreateHighlight();
        }

        public static void ReturnHighlight(TutorialHighlightUI highlight)
        {
            if (highlight != null)
            {
                highlight.Reset();
                highlight.Hide();
                highlightPool.Enqueue(highlight);
            }
        }

        private static TutorialHighlightUI CreateHighlight()
        {
            if (highlightPrefab != null)
            {
                var go = Object.Instantiate(highlightPrefab, poolContainer);
                return go.GetComponent<TutorialHighlightUI>();
            }

            // 无预制体时动态创建
            var highlightObj = new GameObject("TutorialHighlight");
            if (poolContainer != null)
                highlightObj.transform.SetParent(poolContainer, false);

            var rectTransform = highlightObj.AddComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;

            highlightObj.AddComponent<CanvasGroup>();
            return highlightObj.AddComponent<TutorialHighlightUI>();
        }

        #endregion

        public static void Clear()
        {
            while (arrowPool.Count > 0)
            {
                var arrow = arrowPool.Dequeue();
                if (arrow != null)
                    Object.Destroy(arrow.gameObject);
            }

            while (popupPool.Count > 0)
            {
                var popup = popupPool.Dequeue();
                if (popup != null)
                    Object.Destroy(popup.gameObject);
            }

            while (highlightPool.Count > 0)
            {
                var highlight = highlightPool.Dequeue();
                if (highlight != null)
                    Object.Destroy(highlight.gameObject);
            }
        }
    }
}
