namespace ECS.Object.Module
{
    using ECS.Object.Data;
    using UnityEngine;
    using System;

    public interface IObjectControl
    {
        int ControlType { get; }
        ValueTuple<bool, Vector3> CheckControl(IObjectControlData controlData);
    }

    public abstract class ObjectControl<T> : IObjectControl where T : class, IObjectControlData
    {
        public int ControlType { get { return ObjectConstant.DEFAULT_KEYBOARD_CONTROL_TYPE; } }
        
        public ValueTuple<bool, Vector3> CheckControl(IObjectControlData controlData)
        {
            return OnCheckControl(controlData as T);
        }

        public abstract ValueTuple<bool, Vector3> OnCheckControl(T controlData);
    }
}