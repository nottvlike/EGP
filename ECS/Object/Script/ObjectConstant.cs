namespace ECS
{
    using ECS.Common;

    [MergeConstant]
    internal class ObjectConstant
    {
        public const string OBJECT_UNIT_TYPE_NAME = "Object";
        public const string OBJECT_MODULE_GROUP_NAME = "Object";
        
        public const string BUFF_MODULE_GROUP_NAME = "Buff";
        public const string STATE_MODULE_GROUP_NAME = "State";
        public const string SYNC_MODULE_GROUP_NAME = "Sync";
        public const string CONTROL_MODULE_GROUP_NAME = "Control";

        public const int DEFAULT_KEYBOARD_CONTROL_TYPE = 1;
        public const int DEFAULT_UI_CONTROL_TYPE = 1;
    }
}