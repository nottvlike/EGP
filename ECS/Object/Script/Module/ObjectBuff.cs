namespace ECS.Object.Module
{
    using GUnit = ECS.Unit.Unit;
    using ECS.Module;
    using ECS.Data;
    using ECS.Object.Data;
    using UniRx;
    using System;

    public abstract class ObjectBuff<T> : Module where T : ObjectBuffData
    {
        public override int Group { get; protected set; } 
            = WorldManager.Instance.Module.TagToModuleGroupType(ObjectConstant.BUFF_MODULE_GROUP_NAME);

        public ObjectBuff()
        {
            RequiredDataList = new Type[]
            {
                typeof(T)
            };
        }

        protected override void OnAdd(GUnit unit)
        {
            var buffData = GetBuffData(unit);
            var unitData = unit.GetData<UnitData>();
            buffData.stateTypeProperty.Subscribe(_ => {
                if (_ == BuffStateType.Start)
                {
                    OnStart(unit);
                }
                else if (_ == BuffStateType.Stop)
                {
                    OnStop(unit);
                }
            }).AddTo(unitData.disposable);
        }

        protected override void OnRemove(GUnit unit)
        {
            var buffData = GetBuffData(unit);
            buffData.stateTypeProperty.Value = BuffStateType.Stop;
        }

        protected ObjectBuffData GetBuffData(GUnit unit)
        {
            return unit.GetData<T>();
        }

        protected abstract void OnStart(GUnit unit);
        protected abstract void OnStop(GUnit unit);
    }
}