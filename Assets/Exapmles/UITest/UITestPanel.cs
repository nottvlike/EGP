namespace Game.UI
{
    using ECS.UI;
    using ECS.UI.Data;
    using ECS.UI.Module;
    using System;
    using GUnit = ECS.Unit.Unit;
    using UniRx;

    public class UITestPanel : UIModule
    {
        public UITestPanel()
        {
            RequiredDataList = new Type[]{
                typeof(UITestPanelData),
                typeof(PanelData),
            };
        }

        protected override void OnInit(GUnit unit)
        {
            var uiTestPanelData = unit.GetData<Panel>() as UITestPanelData;
            uiTestPanelData.showSimpleTipsPanelButton.onClick.RemoveAllListeners();
            uiTestPanelData.showSimpleTipsPanelButton.onClick.AddListener(ShowSimpleTipsPanel);
        }
        
        void ShowSimpleTipsPanel()
        {
            UIManager.Show("Prefabs/UI/SimpleTipsPanel", false, "This is a test!");
            Observable.Timer(TimeSpan.FromSeconds(6)).Subscribe(_ =>
            {
                UIManager.Hide("Prefabs/UI/SimpleTipsPanel");
            });
        }
    }
}