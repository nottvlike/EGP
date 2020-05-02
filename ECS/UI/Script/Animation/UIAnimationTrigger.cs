namespace UIAnimation
{
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;
    using System;
    using System.Collections.Generic;
    using ECS.Data;

    public enum AnimationTriggerType
    {
        None,
        Auto,
        PointerDownEvent,
        PointerUpEvent,
        PointerEnterEvent,
        PointerExitEvent,
    }

    public sealed class UIAnimationTrigger : MonoBehaviour
    {
        [SerializeField]
        Animator animator;

        [SerializeField]
        string groupName;
        [SerializeField]
        AnimationTriggerType triggerType;
        [SerializeField]
        List<AnimationValueInfo> animationValueInfoList;

        public UnityEvent onPlay;
        public UnityEvent onComplete;
        public string GroupName => groupName;
        public AnimationTriggerType TriggerType => triggerType;

        void OnEnable()
        {
            if (triggerType == AnimationTriggerType.Auto)
            {
                Play();
            }
        }

       public void OnPointerDown(PointerEventData data)
        {
            if (triggerType == AnimationTriggerType.PointerDownEvent)
            {
                Play();
            }
        }

        public void OnPointerUp(PointerEventData data)
        {
            if (triggerType == AnimationTriggerType.PointerUpEvent)
            {
                Play();
            }
        }

        public void OnPointerEnter(PointerEventData data)
        {
            if (triggerType == AnimationTriggerType.PointerEnterEvent)
            {
                Play();
            }
        }

        public void OnPointerExit(PointerEventData data)
        {
            if (triggerType == AnimationTriggerType.PointerExitEvent)
            {
                Play();
            }
        }

        public void OnAnimationPlay()
        {
            onPlay.Invoke();
        }

        public void OnAnimationCommpleted()
        {
            onComplete.Invoke();
        }

        public void Play()
        {
            foreach (var animationValueInfo in animationValueInfoList)
            {
                switch (animationValueInfo.type)
                {
                    case AnimationValueType.Bool:
                        animator.SetBool(animationValueInfo.name, Convert.ToBoolean(animationValueInfo.value));
                        break;
                    case AnimationValueType.Float:
                        animator.SetFloat(animationValueInfo.name, (float)Convert.ToDouble(animationValueInfo.value));
                        break;
                    case AnimationValueType.Integer:
                        animator.SetInteger(animationValueInfo.name, Convert.ToInt32(animationValueInfo.value));
                        break;
                    case AnimationValueType.Trigger:
                        animator.SetTrigger(animationValueInfo.name);
                        break;
                    case AnimationValueType.ResetTrigger:
                        animator.ResetTrigger(animationValueInfo.name);
                        break;
                }
            }
        }
    }
}