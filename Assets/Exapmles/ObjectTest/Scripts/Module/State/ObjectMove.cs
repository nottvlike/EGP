namespace Game.ObjectTest.Module
{
    using ECS.Module;
    using ECS.Data;
    using GUnit = ECS.Unit.Unit;
    using Game.ObjectTest.Data;
    using UnityEngine;
    using UniRx;

    public sealed class ObjectMove : ObjectState
    {
        public override int Id { get; } = ObjectTestConstant.STATE_MOVE;

        Vector2 _movePosition;
        protected override void OnStart(GUnit unit, ObjectStateData stateData)
        {
            var moveSpeedData = unit.GetData<ObjectMoveSpeedData>();
            var assetData = unit.GetData<AssetData>();
            var animator = assetData.GetComponent<Animator>();
            var rigidbody = assetData.GetComponent<Rigidbody2D>();
            animator.SetBool(ObjectTestConstant.ANIMATOR_PARAM_MOVE, true);

            var moveParamData = unit.GetData<ObjectMoveParamData>();
            moveParamData.moveDispose = ObjectSyncServer.EverySyncUpdate().Subscribe(_ =>
            {
                var deltaX = stateData.param.x * moveSpeedData.allValue * ObjectTestConstant.OBJECTTEST_FIXED_MSECOND;
                deltaX /= ObjectTestConstant.MSECOND_TO_SECOND * ObjectTestConstant.MSECOND_TO_SECOND;

                _movePosition.x = deltaX;
                rigidbody.MovePosition(rigidbody.position + _movePosition);
            });
        }

        protected override void OnStop(GUnit unit, ObjectStateData stateData)
        {
            var moveParamData = unit.GetData<ObjectMoveParamData>();
            moveParamData.moveDispose?.Dispose();
            moveParamData.moveDispose = null;

            var assetData = unit.GetData<AssetData>();
            var animator = assetData.GetComponent<Animator>();
            animator.SetBool(ObjectTestConstant.ANIMATOR_PARAM_MOVE, false);
        }

        protected override void OnFinish(GUnit unit, ObjectStateData stateData)
        {
            OnStop(unit, stateData);
        }

    }
}