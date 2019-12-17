namespace Tween
{
    using UnityEngine;
    using DG.Tweening;
    using DTween = DG.Tweening.Tween;

    public enum TweenTriggerType
    {
        None,
        Auto,
        PointerDownEvent,
        PointerUpEvent,
        PointerEnterEvent,
        PointerExitEvent,
    }

    public abstract partial class UITween : MonoBehaviour
    {
        [SerializeField]
        protected TweenTriggerType triggerType;

        [SerializeField]
        protected string groupName;
        [SerializeField]
        protected float duration;
        [SerializeField]
        protected Ease ease;
        [SerializeField]
        protected LoopType loopType;
        [SerializeField]
        protected int loops = 1;

        public TweenTriggerType TriggerType => triggerType;

        public string GroupName => groupName;

        public bool IsLoop => loops == -1;
        
        public DTween Prepare()
        {
            return GetTween().SetEase(ease).SetLoops(loops, loopType);
        }

        protected abstract DTween GetTween();

        #region tween

        DTween _tween;
        protected void Play()
        {
            if (IsPlaying())
            {
                return;
            }

            if (!IsValid())
            {
                _tween = Prepare();
            }
            _tween.Play();
        }

        protected bool IsValid()
        {
            return _tween != null && _tween.active;
        }

        protected bool IsPaused()
        {
            return IsValid() && !_tween.IsPlaying();
        }

        protected bool IsPlaying()
        {
            return IsValid() && _tween.IsPlaying();
        }

        protected void Pause()
        {
            if (_tween != null && _tween.IsPlaying())
            {
                _tween.Pause();
            }
        }

        protected void Kill()
        {
            if (_tween != null)
            {
                _tween.Kill(true);
                _tween = null;
            }
        }

        #endregion
    }
}