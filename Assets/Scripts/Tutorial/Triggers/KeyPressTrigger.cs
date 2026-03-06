using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TutorialSystem
{
    /// <summary>
    /// 按键触发器
    /// </summary>
    [Serializable]
    public class KeyPressTrigger : TutorialTriggerBase
    {
        public override string TriggerName => "按键触发";

        [SerializeField]
        [Tooltip("是否允许任意键触发")]
        private bool anyKey = false;

        [SerializeField]
        [Tooltip("触发按键")]
        private KeyCode triggerKey = KeyCode.Space;

        public bool AnyKey => anyKey;

        public override void Update()
        {
            base.Update();

            if (!IsActivated || isTriggered) return;

            if (anyKey)
            {
                if (Input.anyKeyDown)
                {
                    Trigger();
                }
            }
            else
            {
                if (triggerKey == KeyCode.Mouse0 && IsPointerOverButton())
                {
                    return;
                }

                if (Input.GetKeyDown(triggerKey))
                {
                    Trigger();
                }
            }
        }

        private bool IsPointerOverButton()
        {
            EventSystem eventSystem = EventSystem.current;
            if (eventSystem == null)
            {
                return false;
            }

            PointerEventData pointerData = new PointerEventData(eventSystem)
            {
                position = Input.mousePosition
            };

            List<RaycastResult> results = new List<RaycastResult>();
            eventSystem.RaycastAll(pointerData, results);

            for (int i = 0; i < results.Count; i++)
            {
                GameObject hitObject = results[i].gameObject;
                if (hitObject != null && hitObject.GetComponentInParent<Button>() != null)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
