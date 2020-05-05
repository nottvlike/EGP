namespace Game.ObjectTest.Factory
{
    using GUnit = ECS.Unit.Unit;
    using ECS.Data;
    using ECS.Factory;
    using ECS.Module;
    using ECS.Common;
    using ECS;
    using Game.ObjectTest.Data;
    using Game.ObjectTest.Module;
    using UnityEngine;

    public static class ObjectTestFactory
    {
        public static void InitBuffModuleList(this UnitFactory factory)
        {
            if (ObjectBuffModuleDict.Count() > 0)
            {
                return;
            }

            var slowDownBuff = new ObjectSlowDownBuff();
            ObjectBuffModuleDict.Set(slowDownBuff.Id, slowDownBuff);
        }

        public static void InitObjectControlModule(this UnitFactory factory)
        {
            if (ObjectControlModuleDict.Count() > 0)
            {
                return;
            }

            var moveLeftControl = new MoveLeftControl();
            ObjectControlModuleDict.Set(moveLeftControl.ControlType, moveLeftControl);

            var moveRightControl = new MoveRightControl();
            ObjectControlModuleDict.Set(moveRightControl.ControlType, moveRightControl);
        }

        public static void InitObjectStateModule(this UnitFactory factory)
        {
            if (ObjectStateModuleDict.Count() > 0)
            {
                return;
            }

            var idleState = new ObjectIdle();
            ObjectStateModuleDict.Set(idleState.Id, idleState);

            var moveState = new ObjectMove();
            ObjectStateModuleDict.Set(moveState.Id, moveState);
        }

        public static void CreatePlayer(this UnitFactory factory, GameObject gameObject)
        {
            var moduleMgr = WorldManager.Instance.Module;
            var requiredModuleGroup = moduleMgr.TagToModuleGroupType(ObjectConstant.OBJECT_MODULE_GROUP_NAME)
                | moduleMgr.TagToModuleGroupType(ObjectConstant.CONTROL_MODULE_GROUP_NAME)
                | moduleMgr.TagToModuleGroupType(ObjectConstant.STATE_MODULE_GROUP_NAME)
                | moduleMgr.TagToModuleGroupType(ObjectConstant.SYNC_MODULE_GROUP_NAME);

            var unit = factory.CreateAsset(requiredModuleGroup, gameObject);
            unit.AddData<ObjectSyncData>();

            factory.InitBuffModuleList();

            AttachStateData(unit);
            AttachAttributeData(unit);
            AttachControlData(unit);
            AttachKeyboardControlData(unit);
            AttachBuffData(unit);

            var unitData = unit.GetData<UnitData>();
            unitData.stateTypeProperty.Value = UnitStateType.Init;
        }


        public static void CreateSlowDownTrap(this UnitFactory factory, GameObject gameObject)
        {
            var moduleMgr = WorldManager.Instance.Module;
            var requiredModuleGroup = moduleMgr.TagToModuleGroupType(ObjectConstant.OBJECT_MODULE_GROUP_NAME);
            
            var unit = factory.CreateAsset(requiredModuleGroup, gameObject, false);
            unit.AddData<SlowDownTrapData>();

            var unitData = unit.GetData<UnitData>();
            unitData.stateTypeProperty.Value = UnitStateType.Init;
        }

        static void AttachControlData(GUnit unit)
        {
            var controlStateData = unit.AddData<ObjectControlStateData>();
            controlStateData.stateType.Value = ObjectControlStateType.Start;

            var controlData = Pool.Get<ObjectControlData>();
            controlData.controlType = ObjectTestConstant.MOVE_LEFT_CONTROL_TYPE;
            controlData.stateId = ObjectTestConstant.STATE_MOVE;
            controlData.stateType = ObjectStateType.Start;
            ObjectControlDataDict.Set(unit, controlData.controlType, controlData);

            controlData = Pool.Get<ObjectControlData>();
            controlData.controlType = ObjectTestConstant.MOVE_RIGHT_CONTROL_TYPE;
            controlData.stateId = ObjectTestConstant.STATE_MOVE;
            controlData.stateType = ObjectStateType.Start;
            ObjectControlDataDict.Set(unit, controlData.controlType, controlData);

            WorldManager.Instance.Factory.InitObjectControlModule();
        }

        static void AttachKeyboardControlData(GUnit unit)
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            var controlProcessData = unit.AddData<ObjectKeyboardControlProcessData>();
            var controlDataList = controlProcessData.controlDataList;

            var controlData = Pool.Get<ObjectKeyboardControlData>();
            controlData.controlType = ObjectTestConstant.MOVE_LEFT_CONTROL_TYPE;
            controlData.key = KeyCode.A;
            controlDataList.Add(controlData);

            controlData = Pool.Get<ObjectKeyboardControlData>();
            controlData.controlType = ObjectTestConstant.MOVE_RIGHT_CONTROL_TYPE;
            controlData.key = KeyCode.D;
            controlDataList.Add(controlData);
#endif
        }

        static void AttachStateData(GUnit unit)
        {
            unit.AddData<ObjectStateProcessData>();

            var stateData = Pool.Get<IndependentObjectStateData>();
            stateData.id = ObjectTestConstant.STATE_IDLE;
            stateData.isDefault = true;
            stateData.isLoop = true;
            stateData.priority = 0;
            ObjectStateDataDict.Set(unit, stateData.id, stateData);

            stateData = Pool.Get<IndependentObjectStateData>();
            stateData.id = ObjectTestConstant.STATE_MOVE;
            stateData.isLoop = true;
            stateData.priority = 1;
            ObjectStateDataDict.Set(unit, stateData.id, stateData);

            unit.AddData<ObjectMoveParamData>();

            WorldManager.Instance.Factory.InitObjectStateModule();
        }

        static void AttachAttributeData(GUnit unit)
        {
            var moveSpeedData = unit.AddData<ObjectMoveSpeedData>();
            moveSpeedData.baseValue = 2500;
            moveSpeedData.basePercent = 1f;
        }

        static void AttachBuffData(GUnit unit)
        {
            unit.AddData<ObjectBuffProcessData>();
            WorldManager.Instance.Factory.InitBuffModuleList();
        }
    }
}