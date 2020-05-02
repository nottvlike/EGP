namespace Game.UI.Data
{
    using ECS.Data;
    using UnityEngine;
    using UnityEngine.UI;
    using System.Collections.Generic;

    public class UIPreloadTestPanelData : PanelData
    {
        public GridLayoutGroup grid;
        [HideInInspector]
        public List<GameObject> preloadItemList = new List<GameObject>();
    }
}