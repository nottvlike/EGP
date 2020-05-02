namespace ECS.Module
{
    using System;
    using ECS.Data;

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