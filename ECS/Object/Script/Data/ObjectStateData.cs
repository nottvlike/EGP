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
        public virtual uint id { get; }

        public Vector3 param { get; set; } = Vector3.zero;
        public ReactiveProperty<ObjectStateType> stateTypeProperty { get; } = new ReactiveProperty<ObjectStateType>(ObjectStateType.None);

        public bool IsInUse { get; set; }
        public virtual void Clear()
        {
            param = Vector3.zero;
            stateTypeProperty.Value = ObjectStateType.None;
        }
    }

    public abstract class IndependentObjectStateData : ObjectStateData
    {
        public virtual bool isDefault { get; }
        public virtual bool isLoop { get; }

        public virtual int priority { get; }

        public virtual uint[] excludeIdList { get; private set; } = null;
    }

    public abstract class SupportObjectStateData : ObjectStateData
    {
    }

    public class ObjectStateProcessData : IData, IPoolObject
    {
        public IndependentObjectStateData currentState;
        public List<ObjectStateData> allStateList = new List<ObjectStateData>();

        public bool IsInUse { get; set; }
        public void Clear()
        {
            currentState = null;
            allStateList.Clear();
        }
    }
}