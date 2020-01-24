namespace ECS.UI.Data
{
    using System.Collections.Generic;
    using UnityEngine;
    using ECS.Common;
    using UniRx;
    using GUnit = ECS.Unit.Unit;
    using ECS.Data;

    public class UIData : IData
    {
        public Dictionary<string, GUnit> unitDict = new Dictionary<string, GUnit>();
        public List<string> showedList = new List<string>();
        public GameObject uiRoot;
    }

    public enum PanelStateType
    {
        None,
        Preload,
        Show,
        Hide,
    }

    public class PanelData : IData, IPoolObject
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
}