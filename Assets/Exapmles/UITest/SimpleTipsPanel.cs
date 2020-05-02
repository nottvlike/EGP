namespace Game.UI.Module
{
    using ECS.Data;
    using ECS.Module;
    using Game.UI.Data;
    using System;
    using GUnit = ECS.Unit.Unit;

    public class SimpleTipsPanel : UIModule
    {
        public SimpleTipsPanel()
        {
            RequiredDataList = new Type[]{
                typeof(SimpleTipsPanelData),
                typeof(PanelParamData),
            };
        }
        
        protected override void OnShow(GUnit unit, PanelData panel, params object[] args)
        {
            base.OnShow(unit, panel, args);

            var simpleTipsPanelData = unit.GetData<PanelData>() as SimpleTipsPanelData;
            simpleTipsPanelData.message.text = (string)args[0] ?? string.Empty;
        }

        protected override void OnHide(GUnit unit, PanelData panel)
        {
            base.OnHide(unit, panel);
        }
    }
}