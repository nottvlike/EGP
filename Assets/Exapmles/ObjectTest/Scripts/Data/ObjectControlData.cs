namespace Game.ObjectTest.Data
{
    using ECS.Object.Data;
    using UnityEngine;
    using Game;

    public class ObjectMoveKeyboardData : IObjectKeyboardControlData
    {
        public KeyCode leftKey { get { return KeyCode.A; } }
        public KeyCode rightKey { get { return KeyCode.D; } }

        public int controlType { get {return ObjectTestConstant.MOVE_KEYBOARD_CONTROL_TYPE; } }
        public string stateName { get { return ObjectTestConstant.STATE_MOVE; } }
        public Vector3 stateParam { get; set; }

        public ObjectStateType stateType { get; set; }
    }
}