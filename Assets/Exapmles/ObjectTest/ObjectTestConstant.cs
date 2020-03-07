namespace Game
{
    public class ObjectTestConstant
    {
        public const int OBJECTTEST_FIXED_MSECOND = 17; // 帧率 60，1000 ms / 60
        public const int MSECOND_TO_SECOND = 1000;

        // control type
        public const int MOVE_LEFT_CONTROL_TYPE = 2;
        public const int MOVE_RIGHT_CONTROL_TYPE = 3;

        // state
        public const uint STATE_IDLE = 1001;
        public const uint STATE_MOVE = 1002;

        // animator param
        public const string ANIMATOR_PARAM_IDLE = "Idle";
        public const string ANIMATOR_PARAM_MOVE = "Move";

        // attribute
        public const string ATTRIBUTE_MOVE_SPEED_NAME = "Speed";
    }
}