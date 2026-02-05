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
        private static Queue<TutorialSpriteUI> spritePool = new Queue<TutorialSpriteUI>();

        private static GameObject arrowPrefab;
        private static GameObject popupPrefab;
        private static GameObject highlightPrefab;
        private static GameObject spritePrefab;
        private static Transform poolContainer;

        public static void Initialize(GameObject arrowPrefab, GameObject popupPrefab, Transform container)
        {
            TutorialUIPool.arrowPrefab = arrowPrefab;
            TutorialUIPool.popupPrefab = popupPrefab;
            TutorialUIPool.spritePrefab = spritePrefab;
            poolContainer = container;
        }

        public static void PrewarmPool(int arrowCount = 2, int popupCount = 1, int highlightCount = 1, int spriteCount = 2)
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

            for (int i = 0; i < spriteCount; i++)
            {
                var sprite = CreateSprite();
                if (sprite != null)
                {
                    sprite.Hide();
                    spritePool.Enqueue(sprite);
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

        #region Sprite

        public static TutorialSpriteUI GetSprite()
        {
            if (spritePool.Count > 0)
                return spritePool.Dequeue();
            return CreateSprite();
        }

        public static void ReturnSprite(TutorialSpriteUI sprite)
        {
            if (sprite != null)
            {
                sprite.Hide();
                spritePool.Enqueue(sprite);
            }
        }

        private static TutorialSpriteUI CreateSprite()
        {
            if (spritePrefab != null)
            {
                var go = Object.Instantiate(spritePrefab, poolContainer);
                return go.GetComponent<TutorialSpriteUI>();
            }

            // 无预制体时动态创建
            var spriteObj = new GameObject("TutorialSprite");
            if (poolContainer != null)
                spriteObj.transform.SetParent(poolContainer, false);

            var rectTransform = spriteObj.AddComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.sizeDelta = new Vector2(100, 100);

            spriteObj.AddComponent<CanvasGroup>();
            spriteObj.AddComponent<UnityEngine.UI.Image>();
            return spriteObj.AddComponent<TutorialSpriteUI>();
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

            while (spritePool.Count > 0)
            {
                var sprite = spritePool.Dequeue();
                if (sprite != null)
                    Object.Destroy(sprite.gameObject);
            }
        }
    }
}
