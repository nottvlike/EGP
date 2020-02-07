namespace Game.ObjectTest.Module.Control
{
    using ECS.Object.Data;
    using ECS.Object.Module;
    using Game.ObjectTest.Data;
    using UnityEngine;
    using System;
    using Game;

    public sealed class MoveKeyboardControl : ObjectKeyboardControl<ObjectMoveKeyboardData>
    {
        public override int ControlType { get { return ObjectTestConstant.MOVE_KEYBOARD_CONTROL_TYPE; } }
        
        protected override ValueTuple<bool, Vector3> OnCheckControl(ObjectMoveKeyboardData controlData,
         uint? currentStateId)
        {
            var param = Vector3.zero;
            if (Input.GetKey(controlData.leftKey))
            {
                param = Vector3.left;
            }
            else if (Input.GetKey(controlData.rightKey))
            {
                param = Vector3.right;
            }

            if (param == Vector3.zero)
            {
                controlData.stateType = ObjectStateType.Finish;
            }
            else
            {
                controlData.stateType = ObjectStateType.Start;
            }

            var result = param != controlData.stateParam;
            controlData.stateParam = param;
            return ValueTuple.Create(result, param);
        }
    }
}