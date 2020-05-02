namespace ECS.Data
{
    using System.Collections.Generic;
    using UnityEngine;
    using GUnit = ECS.Unit.Unit;

    public class UIProcessData : IData
    {
        public Dictionary<string, GUnit> unitDict = new Dictionary<string, GUnit>();
        public List<string> showedList = new List<string>();
        public GameObject uiRoot;
    }
}