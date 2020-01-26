namespace ECS.Object.Factory
{
    using UniRx;
    using UnityEngine;
    using ECS.Data;
    using ECS.Factory;
    using ECS.Object;
    using ECS.Object.Data;
    using GUnit = ECS.Unit.Unit;
    using Asset.Factory;

    public static class ObjectFactory
    {
        static GUnit CreateObject(int requiredModuleGroup, GameObject gameObject, ObjectType objectType, int camp)
        {
            var moduleMgr = WorldManager.Instance.Module;
            var factory = WorldManager.Instance.Factory;
            var unit = factory.CreateAsset(requiredModuleGroup
                 | moduleMgr.TagToModuleGroupType(ObjectConstant.OBJECT_MODULE_GROUP_NAME), gameObject);

            var objectData = unit.AddData<ObjectData>();
            objectData.type = objectType;
            objectData.camp = camp;
            
            var unitMgr = WorldManager.Instance.Unit;
            var unitData = unit.GetData<UnitData>();
            unitData.unitType = unitMgr.TagToUnitType(ObjectConstant.OBJECT_UNIT_TYPE_NAME);

            return unit;
        }

        static void AttachAnimator(GUnit unit, GameObject gameObject)
        {
            var stateProcessData = unit.AddData<ObjectStateProcessData>();
        }

        static void AttachAttribute(GUnit unit)
        {
        }
    }
}