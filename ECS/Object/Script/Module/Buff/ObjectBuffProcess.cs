namespace ECS.Object.Module
{
    using GUnit = ECS.Unit.Unit;
    using ECS.Data;
    using ECS.Module;
    using ECS.Object.Data;
    using System;
    using System.Linq;
    using UniRx;

    public sealed class ObjectBuffProcess : Module
    {
        public override int Group { get; protected set; } 
            = WorldManager.Instance.Module.TagToModuleGroupType(ObjectConstant.SYNC_MODULE_GROUP_NAME);

        public ObjectBuffProcess()
        {
            RequiredDataList = new Type[]{
                typeof(ObjectBuffProcessData)
            };
        }

        protected override void OnAdd(GUnit unit)
        {
            var unitData = unit.GetData<UnitData>();
            var processData = unit.GetData<ObjectBuffProcessData>();
            processData.currentBuffList.ObserveAdd().Subscribe(buffData =>
            {
                var buffName = buffData.Value.GetType().ToString();
                var buffModule = processData.allBuffModuleList.Where(_ => _.Name == buffName).FirstOrDefault();
                if (buffModule != null)
                {
                    buffModule.Start(unit, buffData.Value);
                }
            }).AddTo(unitData.disposable);
        }

        public static void AddBuff(GUnit unit, ObjectBuffData buffData)
        {
            var processData = unit.GetData<ObjectBuffProcessData>();
            processData.currentBuffList.Add(buffData);
        }
    }
}