namespace Animation 
{
    using UnityEngine;
    using UnityEngine.EventSystems;
    using System;
    using System.Collections.Generic;

    public enum AnimationValueType
    {
        None,
        Bool,
        Float,
        Integer,
        Trigger,
        ResetTrigger,
    }

    [Serializable]
    public struct AnimationData
    {
        public AnimationValueType type;
        public string name;
        public string value;
    }

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
        public Animator animator;
        public AnimationTriggerType triggerType;
        public List<AnimationData> animationDataList;

        void OnEnable()
        {
            if (triggerType == AnimationTriggerType.Auto)
            {
                DoAnimation();
            }
        }

       public void OnPointerDown(PointerEventData data)
        {
            if (triggerType == AnimationTriggerType.PointerDownEvent)
            {
                DoAnimation();
            }
        }

        public void OnPointerUp(PointerEventData data)
        {
            if (triggerType == AnimationTriggerType.PointerUpEvent)
            {
                DoAnimation();
            }
        }

        public void OnPointerEnter(PointerEventData data)
        {
            if (triggerType == AnimationTriggerType.PointerEnterEvent)
            {
                DoAnimation();
            }
        }

        public void OnPointerExit(PointerEventData data)
        {
            if (triggerType == AnimationTriggerType.PointerExitEvent)
            {
                DoAnimation();
            }
        }

        public void DoAnimation()
        {
            foreach (var animationData in animationDataList)
            {
                switch (animationData.type)
                {
                    case AnimationValueType.Bool:
                        animator.SetBool(animationData.name, Convert.ToBoolean(animationData.value));
                        break;
                    case AnimationValueType.Float:
                        animator.SetFloat(animationData.name, (float)Convert.ToDouble(animationData.value));
                        break;
                    case AnimationValueType.Integer:
                        animator.SetInteger(animationData.name, Convert.ToInt32(animationData.value));
                        break;
                    case AnimationValueType.Trigger:
                        animator.SetTrigger(animationData.name);
                        break;
                    case AnimationValueType.ResetTrigger:
                        animator.ResetTrigger(animationData.name);
                        break;
                }
            }
        }
    }
}