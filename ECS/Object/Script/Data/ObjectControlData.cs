namespace ECS.Object.Data
{
    using ECS.Data;
    using ECS.Common;
    using ECS.Object.Module;
    using ECS.Object.Helper;
    using UnityEngine;
    using System;
    using System.Collections.Generic;
    using UniRx;

    public enum ObjectControlStateType
    {
        None,
        Start,
        Finish
    }

    public enum KeyStateType
    {
        None,
        Down,
        Up
    }

    public class ObjectControlData : IPoolObject
    {
        public int controlType { get; set; }

        public int stateId { get; set; }
        public ObjectStateType stateType { get; set; }

        public Vector3 stateParam { get; set; }

        public ObjectControl objectControl { get; set; }

        public bool IsInUse { get; set; }
        public void Clear()
        {
            controlType = 0;

            stateId = 0;
            stateType = ObjectStateType.None;

            stateParam = Vector3.zero;

            objectControl = null;
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
        public IDisposable checkDispose;

        public bool IsInUse { get; set; }
        public virtual void Clear()
        {
            controlDataList.Clear();
            checkDispose = null;
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
        public CompositeDisposable checkDispose;

        public bool IsInUse { get; set; }
        public virtual void Clear()
        {
            controlDataList.Clear();
            checkDispose = null;
        }
    }

    public class ObjectControlStateData : IData, IPoolObject
    {
        public ReactiveProperty<ObjectControlStateType> stateType = new ReactiveProperty<ObjectControlStateType>();

        public List<ObjectControlData> controlDataList = new List<ObjectControlData>();
        public List<ObjectControl> controlModuleList = new List<ObjectControl>();

        public bool IsInUse { get; set; }
        public virtual void Clear()
        {
            stateType.Value = ObjectControlStateType.None;

            controlDataList.Clear();
            controlModuleList.Clear();
        }
    }
}