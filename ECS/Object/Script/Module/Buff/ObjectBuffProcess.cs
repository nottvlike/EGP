namespace ECS.Module
{
    using GUnit = ECS.Unit.Unit;
    using ECS.Data;
    using ECS.Common;
    using System;

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

        protected override void OnRemove(GUnit unit)
        {
            var processData = unit.GetData<ObjectBuffProcessData>();
            foreach (var buffData in processData.currentBuffDataList)
            {
                Pool.Release(buffData);
            }
            processData.currentBuffDataList.Clear();
        }

        public static void AddBuff(GUnit unit, IBuffData buffData)
        {
            var processData = unit.GetData<ObjectBuffProcessData>();
            var currentBuffDataList = processData.currentBuffDataList;

            var buffModule = ObjectBuffModuleDict.Get(buffData.id);

            var index = currentBuffDataList.IndexOf(buffData);
            if (index != -1)
            {
                buffModule.Update(unit, processData, buffData);
            }
            else
            {
                currentBuffDataList.Add(buffData);
                buffModule.Start(unit, processData, buffData);
            }
        }

        public static void RemoveBuff(GUnit unit, IBuffData buffData)
        {
            var buffModule = ObjectBuffModuleDict.Get(buffData.id);
            var processData = unit.GetData<ObjectBuffProcessData>();
            buffModule.Finish(unit, processData, buffData);
        }
    }
}