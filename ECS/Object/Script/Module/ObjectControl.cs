namespace ECS.Object.Module
{
    using ECS.Object.Data;
    using UnityEngine;
    using System;

    public abstract class ObjectControl
    {
        public virtual int ControlType { get; }

        public virtual int[] ControlTypeList { get; }

        public ValueTuple<bool, Vector3> CheckControl(ObjectControlData controlData,
            ObjectControlStateData controlStateDatata, ObjectStateProcessData stateProcessData)
        {
            return OnCheckControl(controlData, controlStateDatata, stateProcessData.currentState?.id);
        }

        protected abstract ValueTuple<bool, Vector3> OnCheckControl(ObjectControlData controlData,
            ObjectControlStateData controlProcessData, int? currentStateId);
    }
}