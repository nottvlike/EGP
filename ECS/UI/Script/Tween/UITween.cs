namespace Tween
{
    using UnityEngine;
    using DG.Tweening;
    using DTween = DG.Tweening.Tween;

    public abstract class UITween : MonoBehaviour
    {
        [SerializeField]
        protected string groupName;
        [SerializeField]
        protected float duration;
        [SerializeField]
        protected Ease ease;
        [SerializeField]
        protected int loops = 1;

        public string GroupName => groupName;

        public DTween Prepare()
        {
            return GetTween().SetEase(ease).SetLoops(loops);
        }

        protected abstract DTween GetTween();
    }
}