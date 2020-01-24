namespace Game.UI
{
    using ECS.UI.Data;
    using ECS.UI.Module;
    using System;
    using System.Collections.Generic;
    using GUnit = ECS.Unit.Unit;
    using UniRx;
    using Asset;
    using UnityEngine;

    public class UIPreloadTestPanel : UIModule
    {
        public UIPreloadTestPanel()
        {
            RequiredDataList = new Type[]{
                typeof(UIPreloadTestPanelData),
                typeof(PanelData),
            };
        }
        
        protected override void OnPreload(GUnit unit, List<IObservable<Unit>> taskList)
        {
            taskList.Add(AssetManager.Load<GameObject>("Prefabs/UI/PreloadItem").Do(asset => 
            {
                asset.Prespawn(10);
            }).AsUnitObservable());
        }

        protected override void OnShow(GUnit unit, Panel panel, params object[] args)
        {
            var preloadPanelData = panel as UIPreloadTestPanelData;
            var asset = AssetManager.Get<GameObject>("Prefabs/UI/PreloadItem");
            for (var i = 0; i < 10; i++)
            {
                var preloadItem = asset.Spawn();
                preloadItem.transform.SetParent(preloadPanelData.grid.transform);
            }
        }

        protected override void OnHide(GUnit unit, Panel panel)
        {
            var preloadPanelData = panel as UIPreloadTestPanelData;
            foreach (var preloadItem in preloadPanelData.preloadItemList)
            {
                preloadItem.Despawn();
            }

            preloadPanelData.preloadItemList.Clear();
        }
    }
}