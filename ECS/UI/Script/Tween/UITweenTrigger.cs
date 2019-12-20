namespace Tween
{
    using UnityEngine;
    using UnityEngine.EventSystems;
    using DG.Tweening;

    public enum TweenTriggerType
    {
        None,
        Auto,
        PointerDownEvent,
        PointerUpEvent,
        PointerEnterEvent,
        PointerExitEvent,
    }

    partial class UITween : IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler
    {
        [SerializeField]
        TweenTriggerType triggerType;

        public TweenTriggerType TriggerType => triggerType;

        void OnEnable() 
        {
            if (IsPaused() || triggerType == TweenTriggerType.Auto)
            {
                Play();
            }
        }

        void OnDisable() 
        {
            if (IsPlaying())
            {
                Pause();
            }
        }

        public void OnPointerDown(PointerEventData data)
        {
            if (triggerType == TweenTriggerType.PointerDownEvent)
            {
                Play();
            }
        }

        public void OnPointerUp(PointerEventData data)
        {
            if (IsLoop() && triggerType == TweenTriggerType.PointerDownEvent)
            {
                Pause();
            }
            else if (triggerType == TweenTriggerType.PointerUpEvent)
            {
                Play();
            }
        }

        public void OnPointerEnter(PointerEventData data)
        {
            if (triggerType == TweenTriggerType.PointerEnterEvent)
            {
                Play();
            }
        }

        public void OnPointerExit(PointerEventData data)
        {
            if (IsLoop() && triggerType == TweenTriggerType.PointerEnterEvent)
            {
                Pause();
            }
            else if (triggerType == TweenTriggerType.PointerExitEvent)
            {
                Play();
            }
        }
    }
}