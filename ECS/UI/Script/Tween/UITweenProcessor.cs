namespace Tween
{
    using DG.Tweening;
    using System.Collections.Generic;
    using System;
    using System.Linq;
    using UnityEngine;

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
    }

    public class UITweenProcessor : MonoBehaviour
    {
        public List<TweenGroupInfo> tweenGroupInfoList;

        void Reset() 
        {
            tweenGroupInfoList.Clear();

            var tweenList = GetComponentsInChildren<UITween>(true);
            foreach (var tween in tweenList)
            {
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

        public void Play(string groupName, Action onComplete = null)
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

            sequence.Play().OnComplete(() => 
            {
                onComplete?.Invoke();
            });
        }
    }
}