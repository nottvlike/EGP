namespace ECS.Object.Factory
{
    using UniRx;
    using UnityEngine;
    using ECS.Data;
    using ECS.Factory;
    using ECS.Object;
    using ECS.Object.Data;
    using ECS.Object.Config;
    using GUnit = ECS.Unit.Unit;
    using Asset.Factory;

    public static class ObjectFactory
    {
        static GUnit CreateObject(GameObject gameObject, ObjectType objectType, int camp)
        {
            var moduleMgr = WorldManager.Instance.Module;
            var factory = WorldManager.Instance.Factory;
            var requiredModuleGroup = moduleMgr.TagToModuleGroupType(ObjectConstant.OBJECT_MODULE_GROUP_NAME) 
                | moduleMgr.TagToModuleGroupType(Constant.UNIT_MODULE_GROUP_NAME);
            var unit = factory.CreateAsset(requiredModuleGroup, gameObject);

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
            stateProcessData.animator = gameObject.GetComponent<Animator>();
        }

        static void AttachAttribute(GUnit unit)
        {
            var attributeData = unit.AddData<ObjectAttributeData>();

            var attributeConfig = WorldManager.Instance.Config.Get<AttributeConfig>();
            var attributeLength = attributeConfig.attributeInfoList.Length;

            attributeData.attributeInfoList = new ObjectAttributeInfo[attributeLength];
            for (var i = 0; i < attributeLength; i++)
            {
                attributeData.attributeInfoList[i] = new ObjectAttributeInfo() { type = i };
            }
        }

        public static GUnit CreateDecoration(this UnitFactory factory, GameObject gameObject, int camp)
        {
            var unit = CreateObject(gameObject, ObjectType.Decoration, camp);
            AttachAnimator(unit, gameObject);

            var unitData = unit.GetData<UnitData>();
            unitData.stateTypeProperty.Value = UnitStateType.Init;
            return unit;
        }

        public static GUnit CreateTrap(this UnitFactory factory, GameObject gameObject, int camp)
        {
            var unit = CreateObject(gameObject, ObjectType.Decoration, camp);
            AttachAnimator(unit, gameObject);
            AttachAttribute(unit);

            var unitData = unit.GetData<UnitData>();
            unitData.stateTypeProperty.Value = UnitStateType.Init;
            return unit;
        }

        public static GUnit CreateActor(this UnitFactory factory, GameObject gameObject, int camp)
        {
            var unit = CreateObject(gameObject, ObjectType.Decoration, camp);
            AttachAnimator(unit, gameObject);
            AttachAttribute(unit);

            var unitData = unit.GetData<UnitData>();
            unitData.stateTypeProperty.Value = UnitStateType.Init;
            return unit;
        }
    }
}