namespace ECS.UI
{
    using UnityEngine;
    using Tween;
    using UIAnimation;
    using ECS.Data;

    public enum UIAnimationType
    {
        None,
        Tween,
        Animation,
    }

    public abstract class Panel : MonoBehaviour, IData
    {
        [SerializeField]
        PanelMode _panelMode;
        
        [SerializeField]
        int _order;

        [SerializeField]
        UIAnimationType _animationType;
        [SerializeField]
        UIAnimationProcessor _animationProcessor;
        [SerializeField]
        UITweenProcessor _tweenProcessor;

        public PanelMode PanelMode => _panelMode;

        public int Order => _order;

        public UIAnimationType AnimationType => _animationType;
        public UIAnimationProcessor AnimationProcessor=> _animationProcessor;
        public UITweenProcessor TweenProcessor => _tweenProcessor;
    }
}