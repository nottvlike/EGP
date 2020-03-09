namespace Game.ObjectTest.Module.State
{
    using ECS.Object.Module;
    using ECS.Object.Data;
    using GUnit = ECS.Unit.Unit;
    using Asset.Data;
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