namespace Tween
{
    using UnityEngine.EventSystems;

    partial class UITween : IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler
    {
        public void OnPointerDown(PointerEventData data)
        {
            if (triggerType == TweenTriggerType.PointerDownEvent)
            {
                Play();
            }
        }

        public void OnPointerUp(PointerEventData data)
        {
            if (IsLoop && triggerType == TweenTriggerType.PointerDownEvent)
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
            if (IsLoop && triggerType == TweenTriggerType.PointerEnterEvent)
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