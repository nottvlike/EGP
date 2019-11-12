namespace Game.UI
{
    using ECS.UI;
    using System;
    using GUnit = ECS.Entity.Unit;

    public class SimpleTipsPanel : UIModule
    {
        public SimpleTipsPanel()
        {
            RequiredDataList = new Type[]{
                typeof(SimpleTipsPanelData),
                typeof(PanelData),
            };
        }
        
        protected override void OnShow(GUnit unit, Panel panel, params object[] args)
        {
            base.OnShow(unit, panel, args);

            var simpleTipsPanelData = unit.GetData<Panel>() as SimpleTipsPanelData;
            simpleTipsPanelData.message.text = (string)args[0] ?? string.Empty;
        }

        protected override void OnHide(GUnit unit, Panel panel)
        {
            base.OnHide(unit, panel);
        }
    }
}