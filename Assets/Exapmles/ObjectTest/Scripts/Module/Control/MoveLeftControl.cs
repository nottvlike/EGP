namespace Game.ObjectTest.Module.Control
{
    using ECS.Object.Data;
    using ECS.Object.Module;
    using UnityEngine;
    using System;
    using System.Collections.Generic;

    public sealed class MoveLeftControl : ObjectControl
    {
        public override int ControlType { get; } = ObjectTestConstant.MOVE_LEFT_CONTROL_TYPE;

        public override int[] ControlTypeList { get; } = new int[]
        {
            ObjectTestConstant.MOVE_RIGHT_CONTROL_TYPE,
            ObjectTestConstant.MOVE_LEFT_CONTROL_TYPE
        };

        protected override ValueTuple<bool, Vector3> OnCheckControl(ObjectControlData cocntrolData,
            ObjectControlStateData cocntrolStateData, int? currentStateId)
        {
            var param = Vector3.zero;
            var leftState = cocntrolStateData.keyStateDict[ObjectTestConstant.MOVE_LEFT_CONTROL_TYPE];
            if (leftState == KeyStateType.Down)
            {
                cocntrolData.stateType = ObjectStateType.Start;
                param = Vector3.left;
            }
            else if (leftState == KeyStateType.Up)
            {
                cocntrolData.stateType = ObjectStateType.Finish;
            }

            return ValueTuple.Create(leftState != KeyStateType.None, param);
        }
    }
}