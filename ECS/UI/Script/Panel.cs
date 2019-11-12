namespace ECS.UI
{
    using UnityEngine;
    using Tween;
    using ECS.Data;

    public class Panel : MonoBehaviour, IData
    {
        [SerializeField]
        PanelMode _panelMode;
        
        [SerializeField]
        int _order;

        [SerializeField]
        UITweenProcessor _uiTweenProcessor;

        public PanelMode PanelMode => _panelMode;

        public int Order => _order;

        public UITweenProcessor UITweenProcessor => _uiTweenProcessor;
    }
}