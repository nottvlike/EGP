namespace ECS.Object.Data
{
    using ECS.Data;
    using ECS.Common;
    using ECS.Object.Module;
    using ECS.Object.Helper;
    using UnityEngine;
    using System.Collections.Generic;

    public enum ControlStateType
    {
        None,
        Down,
        Up
    }

    public class ObjectControlData : IPoolObject
    {
        public int controlType { get; set; }

        public uint stateId { get; set; }
        public ObjectStateType stateType { get; set; }

        public Vector3 stateParam { get; set; }

        public bool IsInUse { get; set; }
        public void Clear()
        {
            controlType = 0;

            stateId = 0;
            stateType = ObjectStateType.None;

            stateParam = Vector3.zero;
        }
    }

    public class ObjectKeyboardControlData : IPoolObject
    {
        public virtual int controlType { get; set; }
        public virtual KeyCode key { get; set; }

        public bool IsInUse { get; set; }
        public void Clear()
        {
            controlType = 0;
            key = KeyCode.None;
        }
    }

    public class ObjectKeyboardControlProcessData : IData, IPoolObject
    {
        public List<ObjectKeyboardControlData> controlDataList = new List<ObjectKeyboardControlData>();

        public bool IsInUse { get; set; }
        public virtual void Clear()
        {
            controlDataList.Clear();
        }
    }

    public class ObjectUIControlData : IPoolObject
    {
        public int controlType { get; set; }
        public ObjectControlHelper controlHelper { get; set; }

        public bool IsInUse { get; set; }
        public void Clear()
        {
            controlType = 0;
            controlHelper = null;
        }
    }

    public class ObjectUIControlProcessData : IData, IPoolObject
    {
        public List<ObjectUIControlData> controlDataList = new List<ObjectUIControlData>();

        public bool IsInUse { get; set; }
        public virtual void Clear()
        {
            controlDataList.Clear();
        }
    }

    public class ObjectControlStateData : IData, IPoolObject
    {
        public Dictionary<int, ControlStateType> state = new Dictionary<int, ControlStateType>();

        public List<ObjectControlData> controlDataList = new List<ObjectControlData>();
        public List<IObjectControl> controlModuleList = new List<IObjectControl>();

        public bool IsInUse { get; set; }
        public virtual void Clear()
        {
            state.Clear();

            controlDataList.Clear();
            controlModuleList.Clear();
        }
    }
}