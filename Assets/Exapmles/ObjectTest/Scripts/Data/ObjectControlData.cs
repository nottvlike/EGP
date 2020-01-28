namespace Game.ObjectTest.Data
{
    using ECS.Object.Data;
    using UnityEngine;
    using Game;

    public class ObjectMoveLeftData : ObjectKeyboardControlData
    {
        public override KeyCode key { get { return KeyCode.A; } }

        public override ControlStateType controlStateType { get { return ControlStateType.Down; } }
        public override string stateName { get { return ObjectTestConstant.STATE_MOVE; } }
        public override Vector3 stateParam { get { return Vector3.left; } }

        public override ObjectStateType stateType { get{ return ObjectStateType.Start; } }
    }

    public class ObjectFinishMoveLeftData : ObjectKeyboardControlData
    {
        public override KeyCode key { get { return KeyCode.A; } }

        public override ControlStateType controlStateType { get { return ControlStateType.Up; } }
        public override string stateName { get { return ObjectTestConstant.STATE_MOVE; } }
        public override Vector3 stateParam { get { return Vector3.zero; } }
        public override ObjectStateType stateType { get{ return ObjectStateType.Finish; } }
    }

    public class ObjectMoveRightData : ObjectKeyboardControlData
    {
        public override KeyCode key { get { return KeyCode.D; } }

        public override ControlStateType controlStateType { get { return ControlStateType.Down; } }
        public override string stateName { get { return ObjectTestConstant.STATE_MOVE; } }
        public override Vector3 stateParam { get { return Vector3.right; } }
        public override ObjectStateType stateType { get{ return ObjectStateType.Start; } }
    }

    public class ObjectFinishMoveRightData : ObjectKeyboardControlData
    {
        public override KeyCode key { get { return KeyCode.D; } }

        public override ControlStateType controlStateType { get { return ControlStateType.Up; } }
        public override string stateName { get { return ObjectTestConstant.STATE_MOVE; } }
        public override Vector3 stateParam { get { return Vector3.zero; } }
        public override ObjectStateType stateType { get{ return ObjectStateType.Finish; } }
    }
}