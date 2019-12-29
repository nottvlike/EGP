namespace Tween
{
    using DG.Tweening;
    using System.Collections.Generic;
    using System;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.Events;
    using UniRx;

    [Serializable]
    public struct TweenInfo
    {
        public UITween tween;
        public float delay;
    }

    [Serializable]
    public struct TweenGroupInfo
    {
        public string groupName;
        public List<TweenInfo> tweenInfoList;
        public UnityEvent onPlay;
        public UnityEvent onComplete;
    }

    public class UITweenProcessor : MonoBehaviour
    {
        public List<TweenGroupInfo> tweenGroupInfoList;

        ISubject<string> _playSubject = new Subject<string>();

        void Reset() 
        {
            tweenGroupInfoList.Clear();

            var tweenList = GetComponentsInChildren<UITween>(true);
            foreach (var tween in tweenList)
            {
                if (tween.TriggerType != TweenTriggerType.None)
                {
                    continue;
                }

                var tweenGroupInfo = tweenGroupInfoList.Where(_ => _.groupName == tween.GroupName).FirstOrDefault();
                if (tweenGroupInfo.tweenInfoList == null)
                {
                    tweenGroupInfo.groupName = tween.GroupName;
                    tweenGroupInfo.tweenInfoList = new List<TweenInfo>();
                    tweenGroupInfoList.Add(tweenGroupInfo);
                }

                tweenGroupInfo.tweenInfoList.Add(new TweenInfo()
                {
                    tween = tween,
                    delay = 0f
                });
            }
        }

        // for unity event in inspector
        public void Play(string groupName)
        {
            PlayImpl(groupName);
        }

        public void Play(string groupName, Action onComplete)
        {
            PlayImpl(groupName, () => 
            {
                onComplete?.Invoke();
            });
        }

        public IObservable<Unit> PlayAsObserable(string groupName)
        {
            PlayImpl(groupName, () => 
            {
                _playSubject.OnNext(groupName);
            });
            return _playSubject.Where(_ => _ == groupName).First().AsUnitObservable();
        }

        void PlayImpl(string groupName, Action onComplete = null)
        {
            var sequence = DOTween.Sequence();
            var tweenGroupInfo = tweenGroupInfoList.Where(_ => _.groupName == groupName).FirstOrDefault();
            if (tweenGroupInfo.tweenInfoList != null)
            {
                foreach (var tweenInfo in tweenGroupInfo.tweenInfoList)
                {
                    var tween = tweenInfo.tween.Prepare();
                    if (tween == null)
                        continue;

                    sequence.Insert(tweenInfo.delay, tween);
                }
            }

            tweenGroupInfo.onPlay?.Invoke();

            sequence.Play().OnComplete(() => 
            {
                tweenGroupInfo.onComplete?.Invoke();
                onComplete?.Invoke();
            });
        }
    }
}