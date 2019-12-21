namespace Game.UI
{
    using ECS.UI;
    using System;
    using GUnit = ECS.Entity.Unit;
    using UnityEngine;
    using UniRx;

    public class UITweenTriggerTestPanel : UIModule
    {
        public UITweenTriggerTestPanel()
        {
            RequiredDataList = new Type[]{
                typeof(UITweenTriggerTestPanelData),
                typeof(PanelData),
            };
        }

        protected override void OnInit(GUnit unit)
        {
            var tweenTestPanel = unit.GetData<Panel>() as UITweenTriggerTestPanelData;
            tweenTestPanel.backButton.onClick.RemoveAllListeners();
            tweenTestPanel.backButton.onClick.AddListener(OnBackClick);
        }

        void OnBackClick()
        {
            UIManager.Show("Prefabs/UI/UITestPanel", false, null).Subscribe();
        }
    }
}