namespace Tween
{
    using DG.Tweening;
    using DTween = DG.Tweening.Tween;
    using UnityEngine;

    public sealed class UILocalMoveTween : UITween
    {
        [SerializeField]
        Vector3 from;
        [SerializeField]
        Vector3 to;

        protected override DTween GetTween()
        {
            transform.localPosition = from;
            return transform.DOLocalMove(to, duration);
        }
    }
}