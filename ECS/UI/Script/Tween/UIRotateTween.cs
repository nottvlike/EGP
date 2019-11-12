namespace Tween
{
    using DG.Tweening;
    using DTween = DG.Tweening.Tween;
    using UnityEngine;

    public sealed class UIRotateTween : UITween
    {
        [SerializeField]
        Vector3 from;
        [SerializeField]
        Vector3 to;

        protected override DTween GetTween()
        {
            transform.rotation = Quaternion.Euler(from);
            return transform.DORotate(to, duration);
        }
    }
}