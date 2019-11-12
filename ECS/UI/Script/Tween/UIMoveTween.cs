namespace Tween
{
    using DG.Tweening;
    using DTween = DG.Tweening.Tween;
    using UnityEngine;

    public sealed class UIMoveTween : UITween
    {
        [SerializeField]
        Vector3 from;
        [SerializeField]
        Vector3 to;

        protected override DTween GetTween()
        {
            transform.position = from;
            return transform.DOMove(to, duration);
        }
    }
}