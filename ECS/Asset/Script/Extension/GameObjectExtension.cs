namespace ECS.Module
{
    using UnityEngine;
    using ECS.Data;
    using ECS.Module;
    using ECS;

    public static class GameObjectExtension
    {
        static WorldManager worldMgr => WorldManager.Instance;

        public static void Prespawn(this GameObject asset, int count)
        {
            AssetProcess.Prespawn(asset, count);
        }

        public static GameObject Spawn(this GameObject asset, uint unitId = 0)
        {
            var spawn = AssetProcess.Spawn<GameObject>(asset);
            var assetData = spawn.GetComponent<AssetData>();

            if (unitId != 0)
            {
                if (assetData == null)
                {
                    assetData = spawn.AddComponent<AssetData>();
                }
                
                assetData.isSpawned = true;
                assetData.unitId = unitId;

                var unit = worldMgr.Unit.GetUnit(unitId);
                unit.AddData(assetData);
            }
            else
            {
                if (assetData != null)
                {
                    assetData.isSpawned = true;
                }
            }
            return spawn;
        }

        public static void Despawn(this GameObject gameObject)
        {
            var assetData = gameObject.GetComponent<AssetData>();
            if (assetData != null && assetData.unitId != 0)
            {
                var unitId = assetData.unitId;
                assetData.unitId = 0;

                var unit = worldMgr.Unit.GetUnit(unitId);
                var unitData = unit.GetData<UnitData>();
                unitData.stateTypeProperty.Value = UnitStateType.Destroy;
            }
            else
            {
                if (assetData != null)
                {
                    assetData.isSpawned = false;
                }

                AssetProcess.Despawn(gameObject);
            }
        }
    }
}