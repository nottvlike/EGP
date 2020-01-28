namespace ECS.Object.Data
{
    using ECS.Data;
    using ECS.Common;
    using UnityEngine;
    using System.Collections.Generic;

    public enum ControlStateType
    {
        None,
        Down,
        Up
    }

    public abstract class ObjectControlData : IPoolObject
    {
        public virtual ControlStateType controlStateType { get; }
        public virtual string stateName { get; }
        public virtual ObjectStateType stateType { get; }
        public virtual Vector3 stateParam { get; }

        public bool IsInUse { get; set; }
        public virtual void Clear()
        {
        }
    }

    public abstract class ObjectKeyboardControlData : ObjectControlData
    {
        public virtual int mouseButton { get; } = -1;
        public virtual KeyCode key { get; }
    }

    public class ObjectKeyboardControlProcessData : IData, IPoolObject
    {
        public List<ObjectKeyboardControlData> controlDataList = new List<ObjectKeyboardControlData>();

        public bool IsInUse { get; set; }
        public virtual void Clear()
        {
            foreach (var controlData in controlDataList)
            {
                Pool.Release(controlData);
            }
            controlDataList.Clear();
        }
    }
}