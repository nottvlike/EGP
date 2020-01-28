namespace ECS.Object.Module
{
    using ECS.Object.Data;
    using UnityEngine;
    using System;

    public interface IObjectKeyboardControl
    {
        int ControlType { get; }
        ValueTuple<bool, Vector3> UpdateKeyboardControl(ObjectKeyboardControlData controlData);
    }

    public abstract class ObjectKeyboardControl<T> : IObjectKeyboardControl where T : ObjectKeyboardControlData
    {
        public virtual int ControlType { get { return ObjectConstant.DEFAULT_KEYBOARD_CONTROL_TYPE; } }
        public abstract ValueTuple<bool, Vector3> UpdateKeyboardControl(ObjectKeyboardControlData controlData);
    }
}