namespace Game.ObjectTest.Module.Control
{
    using ECS.Object.Data;
    using ECS.Object.Module;
    using UnityEngine;
    using System;
    using System.Collections.Generic;

    public sealed class MoveRightControl : ObjectControl
    {
        public override int ControlType { get; } = ObjectTestConstant.MOVE_RIGHT_CONTROL_TYPE;

        public override List<int> ControlTypeList { get; } = new List<int>()
        {
            ObjectTestConstant.MOVE_RIGHT_CONTROL_TYPE,
            ObjectTestConstant.MOVE_LEFT_CONTROL_TYPE
        };

        protected override ValueTuple<bool, Vector3> OnCheckControl(ObjectControlStateData cocntrolStateData,
             uint? currentStateId)
        {
            var param = Vector3.zero;
            var rightState = cocntrolStateData.state[ObjectTestConstant.MOVE_RIGHT_CONTROL_TYPE];
            if (rightState == ControlStateType.Down)
            {
                ControlData.stateType = ObjectStateType.Start;
                param = Vector3.right;
            }
            else if (rightState == ControlStateType.Up)
            {
                ControlData.stateType = ObjectStateType.Finish;
            }

            return ValueTuple.Create(rightState != ControlStateType.None, param);
        }
    }
}