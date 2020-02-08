namespace ECS.Object.Module
{
    using GUnit = ECS.Unit.Unit;
    using ECS.Module;
    using ECS.Data;
    using ECS.Object.Data;
    using UniRx;
    using System;
    using Asset.Data;

    public abstract class ObjectState<T> : Module where T : ObjectStateData
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
                    OnStart(unit, stateData);
                }
                else if (_ == ObjectStateType.Stop)
                {
                    OnStop(unit, stateData);
                }
                else if (_ == ObjectStateType.Finish)
                {
                    OnFinish(unit, stateData);
                    stateData.stateTypeProperty.Value = ObjectStateType.None;
                }
            }).AddTo(unitData.disposable);

            stateProcessData.allStateList.Add(stateData);

            OnInit(unit, stateData);

            var independentStateData = stateData as IndependentObjectStateData;
            if (independentStateData != null && independentStateData.isDefault)
            {
                ObjectStateProcess.Start(unit, independentStateData.id, independentStateData.param, false);
            }
        }

        protected override void OnRemove(GUnit unit)
        {
            var stateData = GetStateData(unit);
            var stateProcessData = unit.GetData<ObjectStateProcessData>();
            stateData.stateTypeProperty.Value = ObjectStateType.Finish;
            stateProcessData.allStateList.Remove(stateData);
        }

        protected T GetStateData(GUnit unit)
        {
            return unit.GetData<T>();
        }

        protected virtual void OnInit(GUnit unit, T stateData) { }

        protected abstract void OnStart(GUnit unit, T stateData);
        protected abstract void OnStop(GUnit unit, T stateData);
        protected abstract void OnFinish(GUnit unit, T stateData);
    }
}