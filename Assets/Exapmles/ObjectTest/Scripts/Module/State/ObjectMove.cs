namespace Game.ObjectTest.Module.State
{
    using ECS.Object.Module;
    using ECS.Object.Data;
    using GUnit = ECS.Unit.Unit;
    using Game.ObjectTest.Data;
    using UnityEngine;
    using Asset.Data;
    using UniRx;
    using System;

    public sealed class ObjectMove : ObjectState<ObjectMoveStateData> 
    {
        public ObjectMove()
        {
            RequiredDataList = new Type[]
            {
                typeof(ObjectMoveStateData),
                typeof(ObjectStateProcessData),
                typeof(ObjectMoveSpeedData),
                typeof(AssetData),
            };
        }

        Vector2 _movePosition;
        protected override void OnStart(GUnit unit, ObjectMoveStateData stateData)
        {
            var moveSpeedData = unit.GetData<ObjectMoveSpeedData>();
            var assetData = unit.GetData<AssetData>();
            var animator = assetData.GetComponent<Animator>();
            var rigidbody = assetData.GetComponent<Rigidbody2D>();
            animator.SetBool(ObjectTestConstant.ANIMATOR_PARAM_MOVE, true);
            stateData.moveDispose = ObjectSyncServer.EverySyncUpdate().Subscribe(_ =>
            {
                var deltaX = stateData.param.x * moveSpeedData.allValue * ObjectTestConstant.OBJECTTEST_FIXED_MSECOND;
                deltaX /= ObjectTestConstant.MSECOND_TO_SECOND * ObjectTestConstant.MSECOND_TO_SECOND;

                _movePosition.x = deltaX;
                rigidbody.MovePosition(rigidbody.position + _movePosition);
            });
        }

        protected override void OnStop(GUnit unit, ObjectMoveStateData stateData)
        {
            stateData.moveDispose?.Dispose();
            stateData.moveDispose = null;

            var assetData = unit.GetData<AssetData>();
            var animator = assetData.GetComponent<Animator>();
            animator.SetBool(ObjectTestConstant.ANIMATOR_PARAM_MOVE, false);
        }

        protected override void OnFinish(GUnit unit, ObjectMoveStateData stateData)
        {
            OnStop(unit, stateData);
        }

    }
}