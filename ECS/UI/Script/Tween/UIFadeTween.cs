namespace Tween
{
    using DG.Tweening;
    using DTween = DG.Tweening.Tween;
    using UnityEngine;
    using UnityEngine.UI;
    using ECS.Common;

    public sealed class UIFadeTween : UITween
    {
        [SerializeField]
        float from;
        [SerializeField]
        float to;
        
        Image _image;
        CanvasGroup _canvasGroup;

        void Awake()
        {
            _image = GetComponent<Image>();

            if (_image == null)
            {
                _canvasGroup = GetComponent<CanvasGroup>();
            }

            if (_image == null && _canvasGroup == null)
            {
                Log.E("No component can do fade tween!");
            }
        }

        protected override DTween GetTween()
        {  
            if (_image != null)
            {
                var color = _image.color;
                color.a = from;
                return _image.DOFade(to, duration);
            }

            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = from;
                return _canvasGroup.DOFade(to, duration);
            }

            return null;
        }
    }
}