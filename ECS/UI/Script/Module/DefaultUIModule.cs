namespace ECS.UI
{
    using System;
    
    public sealed class DefaultUIModule : UIModule
    {
        public DefaultUIModule()
        {
            RequiredDataList = new Type[]{
                typeof(DefaultPanel),
                typeof(PanelData),
            };
        }
    }
}