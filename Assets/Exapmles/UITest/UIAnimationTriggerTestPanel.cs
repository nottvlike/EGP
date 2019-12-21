namespace Game.UI
{
    using ECS.UI;
    using System;
    using GUnit = ECS.Entity.Unit;
    using UniRx;

    public class UIAnimationTriggerTestPanel : UIModule
    {
        public UIAnimationTriggerTestPanel()
        {
            RequiredDataList = new Type[]{
                typeof(UIAnimationTriggerTestPanelData),
                typeof(PanelData),
            };
        }

        protected override void OnInit(GUnit unit)
        {
            var animationTestPanelData = unit.GetData<Panel>() as UIAnimationTriggerTestPanelData;
            animationTestPanelData.backButton.onClick.RemoveAllListeners();
            animationTestPanelData.backButton.onClick.AddListener(OnBackClick);
        }

        void OnBackClick()
        {
            UIManager.Show("Prefabs/UI/UITestPanel", false, null).Subscribe();
        }
    }
}