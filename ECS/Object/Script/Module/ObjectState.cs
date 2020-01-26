namespace ECS.Object.Module
{
    using GUnit = ECS.Unit.Unit;
    using ECS.Module;
    using ECS.Data;
    using ECS.Object.Data;
    using UniRx;

    public abstract class ObjectState : Module
    {
        public override int Group { get; protected set; } 
            = WorldManager.Instance.Module.TagToModuleGroupType(ObjectConstant.STATE_MODULE_GROUP_NAME);

        protected override void OnAdd(GUnit unit)
        {
            var stateData = GetStateData(unit);
            var stateProcessData = unit.GetData<ObjectStateProcessData>();
            var unitData = unit.GetData<UnitData>();
            stateData.stateTypeProperty.Subscribe(_ => {
                if (_ == ObjectStateType.Start)
                {
                    OnStart(unit, stateData, stateProcessData);
                }
                else if (_ == ObjectStateType.Stop)
                {
                    OnStop(unit, stateData, stateProcessData);
                }
                else if (_ == ObjectStateType.Finish)
                {
                    OnFinish(unit, stateData, stateProcessData);
                    stateData.stateTypeProperty.Value = ObjectStateType.None;
                }
            }).AddTo(unitData.disposable);

            stateProcessData.allStateList.Add(stateData);

            if (stateData.isDefault)
            {
                ObjectStateProcess.Start(unit, stateData);
            }
        }

        protected override void OnRemove(GUnit unit)
        {
            var stateData = GetStateData(unit);
            var stateProcessData = unit.GetData<ObjectStateProcessData>();
            if (stateData.stateTypeProperty.Value == ObjectStateType.Stop)
            {
                stateProcessData.stopStateList.Remove(stateData);
            }

            stateProcessData.allStateList.Remove(stateData);
            stateData.stateTypeProperty.Value = ObjectStateType.Finish;
        }

        protected abstract ObjectStateData GetStateData(GUnit unit);

        protected abstract void OnStart(GUnit unit, ObjectStateData stateData, ObjectStateProcessData stateProcessData);
        protected abstract void OnStop(GUnit unit, ObjectStateData stateData, ObjectStateProcessData stateProcessData);
        protected abstract void OnFinish(GUnit unit, ObjectStateData stateData, ObjectStateProcessData stateProcessData);
    }
}