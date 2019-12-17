namespace Tween
{
    using UnityEngine;
    using DG.Tweening;
    using DTween = DG.Tweening.Tween;

    partial class UITween
    {
        void OnDestroy() 
        {
            Kill();    
        }

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
    }
}