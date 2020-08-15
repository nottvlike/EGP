namespace ECS.Factory
{
    using GUnit = ECS.Unit.Unit;
    using ECS;
    using ECS.Data;
    using ECS.Module;
    using UniRx;
    using System.Linq;
    using UnityEngine;
    using Asset;

    public static class AssetFactory
    {
        public static void DestroyByUnitType(this UnitFactory factory, int unitType)
        {
            var unitList = WorldManager.Instance.Unit.UnitDIctionary.Values.Where(unit => 
            {
                var unitData = unit.GetData<UnitData>();
                return unitData.unitType == unitType;
            }).ToArray();

            foreach (var unit in unitList)
            {
                var unitData = unit.GetData<UnitData>();
                unitData.stateTypeProperty.Value = UnitStateType.Destroy;
            }
        }

        public static void CreateAssetProcess(this UnitFactory factory, string cdn)
        {
            var moduleMgr = WorldManager.Instance.Module;
            var requiredModuleGroup = moduleMgr.TagToModuleGroupType(AssetConstant.ASSET_MODULE_GROUP_NAME);
            var unit = factory.CreateUnit(requiredModuleGroup);

            var processData = unit.AddData<AssetProcessData>();
            processData.CDN = cdn;

            var unitData = unit.GetData<UnitData>();
            var unitMgr = WorldManager.Instance.Unit;
            unitData.unitType = unitMgr.TagToUnitType(AssetConstant.ASSET_UNIT_TYPE_NAME);
            unitData.tag = AssetConstant.ASSET_CORE_UNIT_NAME;

            unitData.stateTypeProperty.Value = UnitStateType.Init;
        }

        public static GUnit CreateAsset(this UnitFactory factory, int requiredModuleGroup, GameObject asset, bool needSpawned = true)
        {
            var unit = WorldManager.Instance.Factory.CreateUnit(requiredModuleGroup);
            if (needSpawned)
            {
                asset.Spawn(unit.UnitId);
            }
            else
            {
                var assetData = asset.GetComponent<AssetData>();
                if (assetData == null)
                {
                    assetData = asset.AddComponent<AssetData>();
                }
                unit.AddData(assetData);

                assetData.unitId = unit.UnitId;
                assetData.isSpawned = false;
            }

            var unitData = unit.GetData<UnitData>();
            unitData.stateTypeProperty.Subscribe(_ => 
            {
                if (_ == UnitStateType.Destroy)
                {
                    var assetData = unit.GetData<AssetData>();
                    assetData.unitId = 0;
                    if (assetData.isSpawned)
                    {
                        assetData.isSpawned = false;
                        AssetProcess.Despawn(assetData.gameObject);
                    }
                }
            }).AddTo(unitData.disposable);
            return unit;
        }
    }
}