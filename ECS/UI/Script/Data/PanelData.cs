namespace ECS.UI.Data
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

    public abstract class PanelData : MonoBehaviour, IData
    {
        [SerializeField]
        PanelMode _panelMode;
        
        [SerializeField]
        int _order;
        [SerializeField]
        bool _destroyWhenHide;

        [SerializeField]
        UIAnimationType _animationType;
        [SerializeField]
        UIAnimationProcessor _animationProcessor;
        [SerializeField]
        UITweenProcessor _tweenProcessor;

        public PanelMode PanelMode => _panelMode;

        public int Order => _order;
        public bool DestroyWhenHide => _destroyWhenHide;

        public UIAnimationType AnimationType => _animationType;
        public UIAnimationProcessor AnimationProcessor=> _animationProcessor;
        public UITweenProcessor TweenProcessor => _tweenProcessor;
    }
}