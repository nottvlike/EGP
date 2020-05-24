namespace Game.ObjectTest.Module
{
    using GUnit = ECS.Unit.Unit;
    using ECS.Data;
    using ECS.Module;
    using ECS;
    using Game.ObjectTest.Data;
    using System;
    using UniRx;
    using UniRx.Triggers;
    using ECS.Common;

    public sealed class SlowDownTrap : Module
    {
        public override int Group { get; protected set; } 
            = WorldManager.Instance.Module.TagToModuleGroupType(Constant.OBJECT_MODULE_GROUP_NAME);

        public SlowDownTrap()
        {
            RequiredDataList = new Type[]
            {
                typeof(SlowDownTrapData),
                typeof(AssetData)
            };
        }

        protected override void OnAdd(GUnit unit)
        {
            var assetData = unit.GetData<AssetData>();
            var unitData = unit.GetData<UnitData>();
            assetData.gameObject.OnTriggerEnter2DAsObservable().Subscribe(collider =>
            {
                var colliderAssetData = collider.GetComponent<AssetData>();
                if (colliderAssetData != null && colliderAssetData.unitId != 0)
                {
                    var colliderUnit = WorldManager.Instance.Unit.GetUnit(colliderAssetData.unitId);
                    var slowDownBuffData = Pool.Get<ObjectSlowDownBuffData>();
                    slowDownBuffData.value = 0.5f;
                    slowDownBuffData.duration = 2f;
                    
                    ObjectBuffProcess.AddBuff(colliderUnit, slowDownBuffData);
                }
            }).AddTo(unitData.disposable);
        }
    }
}