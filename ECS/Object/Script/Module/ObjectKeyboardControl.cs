namespace ECS.Object.Module
{
    using ECS.Object.Data;
    using UnityEngine;
    using System;

    public interface IObjectKeyboardControl
    {
        int ControlType { get; }
        ValueTuple<bool, Vector3> CheckControl(IObjectKeyboardControlData controlData, ObjectStateProcessData processData);
    }

    public abstract class ObjectKeyboardControl<T> : IObjectKeyboardControl where T : class, IObjectKeyboardControlData
    {
        public virtual int ControlType { get; }
        
        public ValueTuple<bool, Vector3> CheckControl(IObjectKeyboardControlData controlData, ObjectStateProcessData processData)
        {
            return OnCheckControl(controlData as T, processData.currentState?.id);
        }

        protected abstract ValueTuple<bool, Vector3> OnCheckControl(T controlData, uint? currentStateId);
    }

    public sealed class DefaultObjectKeyboardControl : ObjectKeyboardControl<DefaultObjectKeyboardControlData>
    {
        public override int ControlType { get { return ObjectConstant.DEFAULT_KEYBOARD_CONTROL_TYPE; } }

        protected override ValueTuple<bool, Vector3> OnCheckControl(DefaultObjectKeyboardControlData controlData,
             uint? currentStateId)
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