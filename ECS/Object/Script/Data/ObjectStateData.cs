namespace ECS.Object.Data
{
    using ECS.Data;
    using ECS.Common;
    using ECS.Object.Module;
    using System.Collections.Generic;
    using UnityEngine;
    using UniRx;

    public enum ObjectStateType
    {
        None,
        Start,
        Update,
        Stop,
        Finish
    }

    public abstract class ObjectStateData : IPoolObject
    {
        public int id { get; set; }

        public Vector3 param { get; set; }
        public ReactiveProperty<ObjectStateType> stateTypeProperty { get; } = new ReactiveProperty<ObjectStateType>(ObjectStateType.None);

        public ObjectState objectState;

        public bool IsInUse { get; set; }
        public virtual void Clear()
        {
            id = 0;

            param = Vector3.zero;
            stateTypeProperty.Value = ObjectStateType.None;

            objectState = null;
        }
    }

    public class IndependentObjectStateData : ObjectStateData
    {
        public bool isDefault { get; set; }
        public bool isLoop { get; set; }

        public int priority { get; set; }

        public float duration { get; set; }

        public override void Clear()
        {
            base.Clear();

            isDefault = false;
            isLoop = false;

            priority = 0;

            duration = 0f;
        }
    }

    public class SupportObjectStateData : ObjectStateData
    {
    }

    public class ObjectStateProcessData : IData, IPoolObject
    {
        public IndependentObjectStateData currentState;

        public bool IsInUse { get; set; }
        public void Clear()
        {
            currentState = null;
        }
    }
}