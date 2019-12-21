namespace Game.UI
{
    using ECS.UI;
    using System;
    using GUnit = ECS.Entity.Unit;
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

            uiTestPanelData.showTweenTestPanelButton.onClick.RemoveAllListeners();
            uiTestPanelData.showTweenTestPanelButton.onClick.AddListener(ShowTweenTestPanel);

            uiTestPanelData.showAnimationTestPanelButton.onClick.RemoveAllListeners();
            uiTestPanelData.showAnimationTestPanelButton.onClick.AddListener(ShowAnimationTestPanel);
        }
        
        void ShowSimpleTipsPanel()
        {
            UIManager.Show("Prefabs/UI/SimpleTipsPanel", false, "This is a test!").Subscribe();
            Observable.Timer(TimeSpan.FromSeconds(6)).Subscribe(_ =>
            {
                UIManager.Hide("Prefabs/UI/SimpleTipsPanel");
            });
        }

        void ShowTweenTestPanel()
        {
            UIManager.Show("Prefabs/UI/UITweenTriggerTestPanel", false, null).Subscribe();
        }

        void ShowAnimationTestPanel()
        {
            UIManager.Show("Prefabs/UI/UIAnimationTriggerTestPanel", false, null).Subscribe();
        }
    }
}