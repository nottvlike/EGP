namespace ECS.Object.Module
{
    using ECS.Object.Data;
    using UnityEngine;
    using System;
    using System.Collections.Generic;

    public interface IObjectControl
    {
        int ControlType { get; }
        ObjectControlData ControlData { get; }
        List<int> ControlTypeList { get; }

        void Bind(ObjectControlData controlData);

        ValueTuple<bool, Vector3> CheckControl(ObjectControlStateData controlStateData,
            ObjectStateProcessData stateProcessData);
    }

    public abstract class ObjectControl : IObjectControl
    {
        public virtual int ControlType { get; }
        public ObjectControlData ControlData { get; private set; }

        public virtual List<int> ControlTypeList { get; }

        public void Bind(ObjectControlData controlData)
        {
            ControlData = controlData;
        }

        public ValueTuple<bool, Vector3> CheckControl(ObjectControlStateData controlStateDatata,
            ObjectStateProcessData stateProcessData)
        {
            return OnCheckControl(controlStateDatata, stateProcessData.currentState?.id);
        }

        protected abstract ValueTuple<bool, Vector3> OnCheckControl(ObjectControlStateData controlProcessData,
            uint? currentStateId);
    }
}