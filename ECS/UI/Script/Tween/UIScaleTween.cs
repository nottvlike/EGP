namespace Tween
{
    using DG.Tweening;
    using DTween = DG.Tweening.Tween;
    using UnityEngine;

    public sealed class UIScaleTween : UITween
    {
        [SerializeField]
        Vector3 from;
        [SerializeField]
        Vector3 to;

        protected override DTween GetTween()
        {
            transform.localScale = from;
            return transform.DOScale(to, duration);
        }
    }
}