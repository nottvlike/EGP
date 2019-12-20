namespace Tween
{
    using UnityEngine;
    using DG.Tweening;
    using DTween = DG.Tweening.Tween;
    
    public abstract partial class UITween : MonoBehaviour
    {
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

        public string GroupName => groupName;

        public DTween Prepare()
        {
            return GetTween().SetEase(ease).SetLoops(loops, loopType);
        }

        protected abstract DTween GetTween();

        #region tween

        DTween _tween;
        public void Play()
        {
            if (IsPlaying())
            {
                return;
            }

            if (_tween == null)
            {
                _tween = Prepare();
            }

            _tween.OnComplete(() => _tween = null).Play();
        }

        public void Pause()
        {
            if (IsPlaying())
            {
                _tween.Pause();
            }
        }

        public void Kill()
        {
            if (_tween == null)
            {
                return;
            }

            _tween.Kill();
            _tween = null;
        }

        public bool IsValid()
        {
            return _tween != null && _tween.IsActive();
        }

        public bool IsPaused()
        {
            return IsValid() && !_tween.IsPlaying();
        }

        public bool IsPlaying()
        {
            return IsValid() && _tween.IsPlaying();
        }

        public bool IsLoop()
        {
            return IsValid() && loops == -1;
        }
        
        #endregion

    }
}