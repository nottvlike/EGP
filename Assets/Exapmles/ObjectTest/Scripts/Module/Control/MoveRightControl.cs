namespace Game.ObjectTest.Module.Control
{
    using ECS.Object.Data;
    using ECS.Object.Module;
    using GUnit = ECS.Unit.Unit;
    using UnityEngine;
    using System;

    public sealed class MoveRightControl : ObjectControl
    {
        public override int ControlType { get; } = ObjectTestConstant.MOVE_RIGHT_CONTROL_TYPE;

        public override int[] ControlTypeList { get; } = new int[]
        {
            ObjectTestConstant.MOVE_RIGHT_CONTROL_TYPE,
            ObjectTestConstant.MOVE_LEFT_CONTROL_TYPE
        };

        protected override ValueTuple<bool, Vector3> OnCheckControl(GUnit unit, ObjectControlData controlData,
            int? currentStateId)
        {
            var param = Vector3.zero;
            
            var rightState = ObjectControlStateTypeDict.Get(unit, ObjectTestConstant.MOVE_RIGHT_CONTROL_TYPE);
            if (rightState == KeyStateType.Down)
            {
                controlData.stateType = ObjectStateType.Start;
                param = Vector3.right;
            }
            else if (rightState == KeyStateType.Up)
            {
                controlData.stateType = ObjectStateType.Finish;
            }

            return ValueTuple.Create(rightState != KeyStateType.None, param);
        }
    }
}