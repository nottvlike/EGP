namespace ECS.Data
{
    using System.Collections.Generic;
    using UnityEngine;
    using Tween;
    using UIAnimation;
    using ECS.Common;
    using UniRx;

    public enum PanelStateType
    {
        None,
        Preload,
        Show,
        Hide,
    }

    public class PanelParamData : IData, IPoolObject
    {
        public ReactiveProperty<PanelStateType> stateTypeProperty;
        public List<object> paramsList = new List<object>();
        public string assetPath;

        public bool IsInUse { get; set; }
        public void Clear()
        {
            stateTypeProperty.Value = PanelStateType.None;
            paramsList.Clear();
            assetPath = string.Empty;
        }
    }

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