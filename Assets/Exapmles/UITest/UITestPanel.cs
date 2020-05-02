namespace Game.UI.Module
{
    using ECS.Data;
    using ECS.Module;
    using Game.UI.Data;
    using System;
    using GUnit = ECS.Unit.Unit;
    using UniRx;

    public class UITestPanel : UIModule
    {
        public UITestPanel()
        {
            RequiredDataList = new Type[]{
                typeof(UITestPanelData),
                typeof(PanelParamData),
            };
        }

        protected override void OnInit(GUnit unit)
        {
            var uiTestPanelData = unit.GetData<PanelData>() as UITestPanelData;
            uiTestPanelData.showSimpleTipsPanelButton.onClick.RemoveAllListeners();
            uiTestPanelData.showSimpleTipsPanelButton.onClick.AddListener(ShowSimpleTipsPanel);
        }
        
        void ShowSimpleTipsPanel()
        {
            UIProcess.Show("Prefabs/UI/SimpleTipsPanel", false, "This is a test!");
            Observable.Timer(TimeSpan.FromSeconds(6)).Subscribe(_ =>
            {
                UIProcess.Hide("Prefabs/UI/SimpleTipsPanel");
            });
        }
    }
}