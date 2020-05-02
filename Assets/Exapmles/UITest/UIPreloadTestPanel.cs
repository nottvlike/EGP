namespace Game.UI
{
    using ECS.Data;
    using ECS.Module;
    using Game.UI.Data;
    using System;
    using System.Collections.Generic;
    using GUnit = ECS.Unit.Unit;
    using UniRx;
    using UnityEngine;

    public class UIPreloadTestPanel : UIModule
    {
        public UIPreloadTestPanel()
        {
            RequiredDataList = new Type[]{
                typeof(UIPreloadTestPanelData),
                typeof(PanelParamData),
            };
        }
        
        protected override void OnPreload(GUnit unit, List<IObservable<Unit>> taskList)
        {
            taskList.Add(AssetProcess.Load<GameObject>("Prefabs/UI/PreloadItem").Do(asset => 
            {
                asset.Prespawn(10);
            }).AsUnitObservable());
        }

        protected override void OnShow(GUnit unit, PanelData panel, params object[] args)
        {
            var preloadPanelData = panel as UIPreloadTestPanelData;
            var asset = AssetProcess.Get<GameObject>("Prefabs/UI/PreloadItem");
            for (var i = 0; i < 10; i++)
            {
                var preloadItem = asset.Spawn();
                preloadItem.transform.SetParent(preloadPanelData.grid.transform);
            }
        }

        protected override void OnHide(GUnit unit, PanelData panel)
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