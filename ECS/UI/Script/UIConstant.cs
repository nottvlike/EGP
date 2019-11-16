namespace ECS.UI
{
    public enum PanelMode
    {
        None = 0,
        Alone,
        Popup
    }

    internal class UIConstant
    {
        public const int SECOND_TO_MILLISECOND = 1000;

        public const string UI_CORE_UNIT_NAME = "UICore";

        public const string OPEN_TWEEN_NAME = "open";
        public const string CLOSE_TWEEN_NAME = "close";

        public const string UI_ROOT_ASSET_NAME = "Prefabs/UI/UIRoot";
    }
}