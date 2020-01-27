namespace Game.ObjectTest.Module.State
{
    using ECS.Object.Module;
    using ECS.Object.Data;
    using GUnit = ECS.Unit.Unit;
    using Game.ObjectTest.Data;
    using Asset.Data;
    using UnityEngine;
    using System;
    public sealed class ObjectIdle : ObjectState<ObjectIdleStateData> 
    {
        public ObjectIdle()
        {
            RequiredDataList = new Type[]
            {
                typeof(ObjectIdleStateData),
                typeof(ObjectStateProcessData),
                typeof(AssetData),
            };
        }

        protected override void OnStart(GUnit unit, ObjectIdleStateData stateData)
        {
            var assetData = unit.GetData<AssetData>();
            var animator = assetData.GetComponent<Animator>();
            animator.SetBool(ObjectTestConstant.ANIMATOR_PARAM_IDLE, true);
        }

        protected override void OnStop(GUnit unit, ObjectIdleStateData stateData)
        {
            var assetData = unit.GetData<AssetData>();
            var animator = assetData.GetComponent<Animator>();
            animator.SetBool(ObjectTestConstant.ANIMATOR_PARAM_IDLE, false);
        }

        protected override void OnFinish(GUnit unit, ObjectIdleStateData stateData)
        {
            OnStop(unit, stateData);
        }

    }
}