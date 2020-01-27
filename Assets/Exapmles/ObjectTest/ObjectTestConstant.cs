namespace Game
{
    public class ObjectTestConstant
    {
        public const int OBJECTTEST_FIXED_MSECOND = 17; // 帧率 60，1000 ms / 60
        public const int MSECOND_TO_SECOND = 1000;

        // control
        public const string KEYBOARD_CONTROL_MOVE_LEFT = "MoveLeft";
        public const string KEYBOARD_CONTROL_MOVE_RIGHT = "MoveRight";

        // state
        public const string STATE_IDLE = "Idle";
        public const string STATE_MOVE = "Move";

        // animator param
        public const string ANIMATOR_PARAM_IDLE = "Idle";
        public const string ANIMATOR_PARAM_MOVE = "Move";

        // attribute
        public const string ATTRIBUTE_MOVE_SPEED_NAME = "Speed";
        public const string ATTRIBUTE_MOVE_SPEED_DESCRIPTION = "Speed Description";
    }
}