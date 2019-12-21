namespace Animation 
{
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;
    using System;
    using System.Collections.Generic;
    using UniRx;

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
        [SerializeField]
        Animator animator;

        [SerializeField]
        string groupName;
        [SerializeField]
        AnimationTriggerType triggerType;
        [SerializeField]
        List<AnimationData> animationDataList;

        public UnityEvent onPlay;
        public UnityEvent onComplete;
        public string GroupName => groupName;
        public AnimationTriggerType TriggerType => triggerType;

        ISubject<bool> _playSubject;
        ISubject<bool> _completeSubject;

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
            if (_playSubject != null)
            {
                _playSubject.OnNext(true);
                _playSubject.OnCompleted();
                _playSubject = null;
            }
        }

        public void OnAnimationCommpleted()
        {
            if (_completeSubject != null)
            {
                _completeSubject.OnNext(true);
                _completeSubject.OnCompleted();
                _completeSubject = null;
            }
        }

        public void Play()
        {
            if (onComplete.GetPersistentEventCount() > 0)
            {
                if (_completeSubject != null)
                {
                    _completeSubject.OnCompleted();
                    _completeSubject = null;
                }

                _completeSubject = new Subject<bool>();
                _completeSubject.Subscribe(_ => onComplete.Invoke());
            }

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

            if (onPlay.GetPersistentEventCount() > 0)
            {
                if (_playSubject != null)
                {
                    _playSubject.OnCompleted();
                    _playSubject = null;
                }

                _playSubject = new Subject<bool>();
                _playSubject.Subscribe(_ => onPlay.Invoke());
            }
        }
    }
}