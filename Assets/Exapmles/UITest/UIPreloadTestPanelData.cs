namespace Game.UI
{
    using ECS.UI;
    using UnityEngine;
    using UnityEngine.UI;
    using System.Collections.Generic;

    public class UIPreloadTestPanelData : Panel
    {
        public GridLayoutGroup grid;
        [HideInInspector]
        public List<GameObject> preloadItemList = new List<GameObject>();
    }
}