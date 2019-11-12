namespace ECS.UI
{
    using System.Collections.Generic;
    using UnityEngine;
    using ECS.Common;
    using UniRx;
    using GUnit = ECS.Entity.Unit;
    using ECS.Data;

    public class UIData : IData
    {
        public Dictionary<string, GUnit> unitDict = new Dictionary<string, GUnit>();
        public Dictionary<string, Panel> panelDict = new Dictionary<string, Panel>();
        public List<string> showedList = new List<string>();
        public GameObject uiRoot;
    }

    public enum PanelStateType
    {
        None,
        Show,
        Hide,
    }

    public class PanelData : IData, IPoolObject
    {
        public ReactiveProperty<PanelStateType> stateTypeProperty;
        public List<object> paramsList = new List<object>();
        public bool IsInUse { get; set; }
        public void Clear()
        {
            stateTypeProperty.Value = PanelStateType.None;
            paramsList = null;
        }
    }
}