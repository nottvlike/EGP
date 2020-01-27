namespace Game.ObjectTest.Factory
{
    using GUnit = ECS.Unit.Unit;
    using ECS.Data;
    using ECS.Object;
    using ECS.Object.Data;
    using ECS.Factory;
    using ECS;
    using Asset.Factory;
    using Game.ObjectTest.Data;
    using UnityEngine;

    public static class ObjectTestFactory
    {
        public static void CreatePlayer(this UnitFactory factory, GameObject gameObject)
        {
            var moduleMgr = WorldManager.Instance.Module;
            var requiredModuleGroup = moduleMgr.TagToModuleGroupType(ObjectConstant.OBJECT_MODULE_GROUP_NAME)
                | moduleMgr.TagToModuleGroupType(ObjectConstant.KEYBOARD_CONTROL_MODULE_GROUP_NAME)
                | moduleMgr.TagToModuleGroupType(ObjectConstant.STATE_MODULE_GROUP_NAME)
                | moduleMgr.TagToModuleGroupType(ObjectConstant.SYNC_MODULE_GROUP_NAME);

            var unit = factory.CreateAsset(requiredModuleGroup, gameObject);
            unit.AddData<ObjectSyncData>();

            AttachStateData(unit);
            AttachAttributeData(unit);
            AttachControlData(unit);

            var unitData = unit.GetData<UnitData>();
            unitData.stateTypeProperty.Value = UnitStateType.Init;
        }

        static void AttachControlData(GUnit unit)
        {
            unit.AddData<ObjectMoveLeftData>();
            unit.AddData<ObjectMoveRightData>();
            unit.AddData<ObjectFinishMoveLeftData>();
            unit.AddData<ObjectFinishMoveRightData>();
        }

        static void AttachStateData(GUnit unit)
        {
            unit.AddData<ObjectStateProcessData>();

            unit.AddData<ObjectMoveStateData>();
            unit.AddData<ObjectIdleStateData>();
        }

        static void AttachAttributeData(GUnit unit)
        {
            var moveSpeedData = unit.AddData<ObjectMoveSpeedData>();
            moveSpeedData.baseValue = 2500;
            moveSpeedData.basePercent = 1f;
        }
    }
}