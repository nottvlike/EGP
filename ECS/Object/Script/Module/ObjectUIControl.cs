namespace ECS.Object.Module
{
    using ECS.Object.Data;
    using UnityEngine;
    using System;
    using System.Collections.Generic;

    public interface IObjectUIControl
    {
        Type ControlDataType { get; }
        ObjectUIControlData ControlData { get; }
        List<int> ControlTypeList { get; }

        void Bind(ObjectUIControlData controlData);

        ValueTuple<bool, Vector3> CheckControl(ObjectUIControlProcessData controlProcessData,
            ObjectStateProcessData processData);
    }

    public abstract class ObjectUIControl<T> : IObjectUIControl where T : ObjectUIControlData
    {
        public Type ControlDataType { get; } = typeof(T);
        public ObjectUIControlData ControlData { get; private set; }

        public virtual List<int> ControlTypeList { get; }

        public void Bind(ObjectUIControlData controlData)
        {
            ControlData = controlData;
        }

        public ValueTuple<bool, Vector3> CheckControl(ObjectUIControlProcessData controlProcessData,
            ObjectStateProcessData processData)
        {
            return OnCheckControl(controlProcessData, processData.currentState?.id);
        }

        protected abstract ValueTuple<bool, Vector3> OnCheckControl(ObjectUIControlProcessData controlProcessData,
            uint? currentStateId);
    }
}