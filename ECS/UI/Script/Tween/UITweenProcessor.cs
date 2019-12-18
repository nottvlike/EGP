namespace Tween
{
    using DG.Tweening;
    using System.Collections.Generic;
    using System;
    using System.Linq;
    using UnityEngine;
    using ECS.UI;

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

        Dictionary<string, Sequence> _tweenDict = new Dictionary<string, Sequence>();

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

        public void PlayOpen(Action onComplete = null)
        {
            gameObject.SetActive(true);
            Play(UIConstant.OPEN_TWEEN_NAME, () => 
            {
                onComplete?.Invoke();
            });
        }

        public void PlayClose(Action onComplete = null)
        {
            Play(UIConstant.CLOSE_TWEEN_NAME, () => 
            {
                onComplete?.Invoke();
                gameObject.SetActive(false);
            });
        }

        public void Play(string groupName, Action onComplete = null)
        {
            if (_tweenDict.ContainsKey(groupName))
            {
                Kill(groupName);
                _tweenDict.Remove(groupName);
            }

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

            _tweenDict.Add(groupName, sequence);

            sequence.Play().OnComplete(() => 
            {
                _tweenDict.Remove(groupName);
                onComplete?.Invoke();
            });
        }

        public void Pause(string groupName)
        {
            if (IsPlaying(groupName))
            {
                _tweenDict[groupName].Pause();
            }
        }

        public void Kill(string groupName)
        {
            if (!_tweenDict.ContainsKey(groupName))
            {
                return;
            }

            var sequence = _tweenDict[groupName];
            sequence.Kill();
            _tweenDict.Remove(groupName);
        }

        public bool IsValid(string groupName)
        {
            var sequence = _tweenDict[groupName];
            return sequence != null && sequence.IsActive();
        }

        public bool IsPaused(string groupName)
        {
            return IsValid(groupName) && !_tweenDict[groupName].IsPlaying();
        }

        public bool IsPlaying(string groupName)
        {
            return IsValid(groupName) && _tweenDict[groupName].IsPlaying();
        }

        public bool IsLoop(string groupName)
        {
            return IsValid(groupName) && _tweenDict[groupName].Loops() == -1;
        }
    }
}