namespace ECS.UI.Module
{
    using System;
    using ECS.UI.Data;

    public sealed class DefaultUIModule : UIModule
    {
        public DefaultUIModule()
        {
            RequiredDataList = new Type[]{
                typeof(DefaultPanelData),
                typeof(PanelParamData),
            };
        }
    }
}