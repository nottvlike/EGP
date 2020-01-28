namespace ECS.Object.Module
{
    using GUnit = ECS.Unit.Unit;
    using ECS.Module;
    using ECS.Data;
    using ECS.Object.Data;
    using UniRx;
    using UnityEngine;
    using System;

    public sealed class DefaultObjectKeyboardControl : ObjectKeyboardControl<ObjectKeyboardControlData>
    {
        public override ValueTuple<bool, Vector3> UpdateKeyboardControl(ObjectKeyboardControlData controlData)
        {
            if (controlData.mouseButton == -1)
            {
                if ((controlData.controlStateType == ControlStateType.Down 
                    && Input.GetKeyDown(controlData.key))
                    || (controlData.controlStateType == ControlStateType.Up 
                    && Input.GetKeyUp(controlData.key)))
                {
                    return ValueTuple.Create(true, controlData.stateParam);
                }
            }
            else
            {
                if ((controlData.controlStateType == ControlStateType.Down 
                    && Input.GetMouseButtonDown(controlData.mouseButton))
                    || controlData.controlStateType == ControlStateType.Up
                    && Input.GetMouseButtonUp(controlData.mouseButton))
                {
                    return ValueTuple.Create(true, controlData.stateParam);
                }
            }
            return ValueTuple.Create(false, Vector3.zero);
        }
    }
}