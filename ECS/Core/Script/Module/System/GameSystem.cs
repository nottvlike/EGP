namespace ECS.Module
{
    using Data;
    using System;
    using GUnit = ECS.Entity.Unit;
    using UniRx;
    using UnityEngine;

    public sealed class GameSystem : Module
    {
        public override int Group { get; protected set; } 
            = WorldManager.Instance.Module.TagToModuleGroupType(Constant.SYSTEM_MODULE_GROUP_NAME);

        public GameSystem()
        {
            RequiredDataList = new Type[]{
                typeof(SystemData)
            };
        }

        protected override void OnAdd(GUnit unit)
        {
            var systemData = unit.GetData<SystemData>();
            var unitData = unit.GetData<UnitData>();

            Observable.EveryUpdate().Subscribe(_ =>
            {
                var deltaTime = Mathf.FloorToInt(Time.deltaTime * Constant.SECOND_TO_MILLISECOND);
                systemData.deltaTime = deltaTime;
                systemData.time += deltaTime;
                systemData.clientFrame++;

            }).AddTo(unitData.disposable);
        }
    }
}