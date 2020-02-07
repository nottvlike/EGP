namespace ECS.Object.Data
{
    using ECS.Data;
    using ECS.Common;
    using System.Collections.Generic;
    using UnityEngine;
    using UniRx;
    using System;

    public enum ObjectStateType
    {
        None,
        Start,
        Update,
        Stop,
        Finish
    }

    public abstract class ObjectStateData : IData, IPoolObject
    {
        public virtual string name { get; }
        public virtual int priority { get; }
        public virtual bool isLoop { get; }
        public virtual bool isDefault { get; } = false;
        public virtual bool isSupporting { get; } = false;

        public virtual string[] excludeNameList { get; } = null;

        public Vector3 param = Vector3.zero;
        public ReactiveProperty<ObjectStateType> stateTypeProperty = new ReactiveProperty<ObjectStateType>(ObjectStateType.None);

        public bool IsInUse { get; set; }
        public virtual void Clear()
        {
            param = Vector3.zero;
            stateTypeProperty.Value = ObjectStateType.None;
        }
    }

    public class ObjectStateProcessData : IData, IPoolObject
    {
        public ObjectStateData currentState;
        public List<ObjectStateData> supportingStateList = new List<ObjectStateData>();
        public List<ObjectStateData> stopStateList = new List<ObjectStateData>();
        public List<ObjectStateData> allStateList = new List<ObjectStateData>();
        public IDisposable checkFinishDispose;

        public bool IsInUse { get; set; }
        public void Clear()
        {
            currentState = null;
            stopStateList.Clear();
            allStateList.Clear();
            checkFinishDispose = null;
        }
    }
}