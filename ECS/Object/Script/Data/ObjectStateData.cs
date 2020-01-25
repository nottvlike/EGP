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
        Stop,
        Finish
    }

    public abstract class ObjectStateData : IData, IPoolObject
    {
        public virtual string name { get; }
        public virtual int priority { get; }
        public virtual bool isLoop { get; }
        public virtual bool isDefault { get; }

        public ReactiveProperty<ObjectStateType> stateTypeProperty = new ReactiveProperty<ObjectStateType>(ObjectStateType.None);

        public bool IsInUse { get; set; }
        public virtual void Clear()
        {
            stateTypeProperty.Value = ObjectStateType.None;
        }
    }

    public class ObjectStateProcessData : IData, IPoolObject
    {
        public Animator animator;
        public ObjectStateData currentState;
        public List<ObjectStateData> stopStateList = new List<ObjectStateData>();
        public IDisposable checkFinishDispose;

        public bool IsInUse { get; set; }
        public void Clear()
        {
            currentState = null;
            animator = null;
            stopStateList.Clear();
            checkFinishDispose = null;
        }
    }
}