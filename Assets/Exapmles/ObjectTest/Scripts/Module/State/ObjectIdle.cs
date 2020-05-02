namespace Game.ObjectTest.Module
{
    using ECS.Module;
    using ECS.Data;
    using GUnit = ECS.Unit.Unit;
    using UnityEngine;

    public sealed class ObjectIdle : ObjectState
    {
        public override int Id { get; } = ObjectTestConstant.STATE_IDLE;

        protected override void OnStart(GUnit unit, ObjectStateData stateData)
        {
            var assetData = unit.GetData<AssetData>();
            var animator = assetData.GetComponent<Animator>();
            animator.SetBool(ObjectTestConstant.ANIMATOR_PARAM_IDLE, true);
        }

        protected override void OnStop(GUnit unit, ObjectStateData stateData)
        {
            var assetData = unit.GetData<AssetData>();
            var animator = assetData.GetComponent<Animator>();
            animator.SetBool(ObjectTestConstant.ANIMATOR_PARAM_IDLE, false);
        }

        protected override void OnFinish(GUnit unit, ObjectStateData stateData)
        {
            OnStop(unit, stateData);
        }

    }
}