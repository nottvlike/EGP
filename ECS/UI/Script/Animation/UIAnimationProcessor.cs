namespace UIAnimation 
{
    using UnityEngine;
    using UnityEngine.Events;
    using System.Collections.Generic;
    using UniRx;
    using System.Linq;
    using ECS.Common;

    public sealed class UIAnimationProcessor : MonoBehaviour
    {
        public List<UIAnimationTrigger> animationTriggerList;

        void Reset() 
        {
            animationTriggerList.Clear();

            var triggerList = GetComponentsInChildren<UIAnimationTrigger>(true);
            animationTriggerList.AddRange(triggerList.Where(_ => !string.IsNullOrEmpty(_.GroupName)));
        }

        public void Play(string groupName, UnityAction onComplete = null)
        {
            var animationTrigger = animationTriggerList.Where(_ => _.GroupName == groupName).FirstOrDefault();
#if DEBUG
            if (animationTrigger == null)
            {
                Log.E("Failed to find animation of group {0}", groupName);
                return;
            }
#endif

            if (onComplete != null)
            {
                animationTrigger.onComplete.RemoveAllListeners();
                animationTrigger.onComplete.AddListener(onComplete);
            }

            animationTrigger.Play();
        }

        public void OnAnimationPlay(string groupName)
        {
            var animationTrigger = animationTriggerList.Where(_ => _.GroupName == groupName).FirstOrDefault();
            animationTrigger.OnAnimationPlay();
        }

        public void OnAnimationCommpleted(string groupName)
        {
            var animationTrigger = animationTriggerList.Where(_ => _.GroupName == groupName).FirstOrDefault();
            animationTrigger.OnAnimationCommpleted();
        }
    }
}