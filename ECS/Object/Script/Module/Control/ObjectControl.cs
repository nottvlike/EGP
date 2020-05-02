namespace ECS.Module
{
    using GUnit = ECS.Unit.Unit;
    using ECS.Data;
    using UnityEngine;
    using System;

    public abstract class ObjectControl
    {
        public virtual int ControlType { get; }

        public virtual int[] ControlTypeList { get; }

        public ValueTuple<bool, Vector3> CheckControl(GUnit unit, ObjectControlData controlData,
            ObjectStateProcessData stateProcessData)
        {
            return OnCheckControl(unit, controlData, stateProcessData.currentState?.id);
        }

        protected abstract ValueTuple<bool, Vector3> OnCheckControl(GUnit unit, ObjectControlData controlData,
            int? currentStateId);
    }
}