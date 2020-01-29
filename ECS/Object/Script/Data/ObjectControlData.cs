namespace ECS.Object.Data
{
    using ECS.Data;
    using ECS.Common;
    using ECS.Object.Module;
    using UnityEngine;
    using System.Collections.Generic;

    public enum ControlStateType
    {
        None,
        Down,
        Up
    }

    public interface IObjectControlData
    {
        string stateName { get; }
        ObjectStateType stateType { get; }
    }

    public abstract class DefaultObjectControlData : IObjectControlData
    {
        public virtual string stateName { get; }
        public virtual ObjectStateType stateType { get; }

        public virtual ControlStateType controlStateType { get; }
        public virtual Vector3 stateParam { get; }
    }

    public interface IObjectKeyboardControlData : IObjectControlData
    {
        int controlType { get; }
    }

    public abstract class DefaultObjectKeyboardControlData : DefaultObjectControlData, IObjectKeyboardControlData
    {
        public virtual int controlType { get; } = ObjectConstant.DEFAULT_KEYBOARD_CONTROL_TYPE;
        public virtual int mouseButton { get; } = -1;
        public virtual KeyCode key { get; }
    }

    public class ObjectKeyboardControlProcessData : IData, IPoolObject
    {
        public List<IObjectKeyboardControlData> controlDataList = new List<IObjectKeyboardControlData>();
        public List<IObjectKeyboardControl> allControlModuleList = new List<IObjectKeyboardControl>();

        public bool IsInUse { get; set; }
        public virtual void Clear()
        {
            controlDataList.Clear();

            allControlModuleList.Clear();
        }
    }
}