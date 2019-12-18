namespace Tween
{
    using UnityEngine;
    using UnityEngine.EventSystems;

    public enum TweenTriggerType
    {
        None,
        Auto,
        PointerDownEvent,
        PointerUpEvent,
        PointerEnterEvent,
        PointerExitEvent,
    }

    public sealed class UITweenTrigger : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler
    {
        [SerializeField]
        UITweenProcessor processor;

        [SerializeField]
        string groupName;
        [SerializeField]
        TweenTriggerType triggerType;

        void OnEnable() 
        {
            if (processor.IsPaused(groupName) || triggerType == TweenTriggerType.Auto)
            {
                processor.Play(groupName);
            }
        }

        void OnDisable() 
        {
            if (processor.IsPlaying(groupName))
            {
                processor.Pause(groupName);
            }
        }

        public void OnPointerDown(PointerEventData data)
        {
            if (triggerType == TweenTriggerType.PointerDownEvent)
            {
                processor.Play(groupName);
            }
        }

        public void OnPointerUp(PointerEventData data)
        {
            if (processor.IsLoop(groupName) && triggerType == TweenTriggerType.PointerDownEvent)
            {
                processor.Pause(groupName);
            }
            else if (triggerType == TweenTriggerType.PointerUpEvent)
            {
                processor.Play(groupName);
            }
        }

        public void OnPointerEnter(PointerEventData data)
        {
            if (triggerType == TweenTriggerType.PointerEnterEvent)
            {
                processor.Play(groupName);
            }
        }

        public void OnPointerExit(PointerEventData data)
        {
            if (processor.IsLoop(groupName) && triggerType == TweenTriggerType.PointerEnterEvent)
            {
                processor.Pause(groupName);
            }
            else if (triggerType == TweenTriggerType.PointerExitEvent)
            {
                processor.Play(groupName);
            }
        }
    }
}