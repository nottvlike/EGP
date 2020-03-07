namespace Game.ObjectTest.Factory
{
    using GUnit = ECS.Unit.Unit;
    using ECS.Data;
    using ECS.Object;
    using ECS.Object.Data;
    using ECS.Object.Module;
    using ECS.Factory;
    using ECS.Common;
    using ECS;
    using Asset.Factory;
    using Game.ObjectTest.Data;
    using Game.ObjectTest.Module.Buff;
    using Game.ObjectTest.Module.Control;
    using UnityEngine;
    using System.Collections.Generic;

    public static class ObjectTestFactory
    {
        static List<IObjectBuff> _buffModuleList = new List<IObjectBuff>();
        static void InitBuffModuleList()
        {
            _buffModuleList.Add(new ObjectSlowDownBuff());
        }

        public static List<IObjectBuff> GetAllBuffModule(this UnitFactory factory)
        {
            if (_buffModuleList.Count == 0)
            {
                InitBuffModuleList();
            }

            return _buffModuleList;
        }

        static List<IObjectControl> _controlModuleList = new List<IObjectControl>();
        static void InitControlModuleList()
        {
            _controlModuleList.Add(new MoveLeftControl());
            _controlModuleList.Add(new MoveRightControl());
        }

        public static List<IObjectControl> GetAllKeyboardControlModule(this UnitFactory factory)
        {
            if (_controlModuleList.Count == 0)
            {
                InitControlModuleList();
            }

            return _controlModuleList;
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

            if (_buffModuleList.Count == 0)
            {
                InitBuffModuleList();
            }

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
            var controlDataList = controlStateData.controlDataList;

            var controlData = Pool.Get<ObjectControlData>();
            controlData.controlType = ObjectTestConstant.MOVE_LEFT_CONTROL_TYPE;
            controlData.stateId = ObjectTestConstant.STATE_MOVE;
            controlData.stateType = ObjectStateType.Start;
            controlDataList.Add(controlData);

            controlData = Pool.Get<ObjectControlData>();
            controlData.controlType = ObjectTestConstant.MOVE_RIGHT_CONTROL_TYPE;
            controlData.stateId = ObjectTestConstant.STATE_MOVE;
            controlData.stateType = ObjectStateType.Start;
            controlDataList.Add(controlData);

            var factory = WorldManager.Instance.Factory;
            controlStateData.controlModuleList.AddRange(factory.GetAllKeyboardControlModule());
        }

        static void AttachKeyboardControlData(GUnit unit)
        {
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

        static void AttachBuffData(GUnit unit)
        {
            var buffProcessData = unit.AddData<ObjectBuffProcessData>();
            var factory = WorldManager.Instance.Factory;
            buffProcessData.allBuffModuleList.AddRange(factory.GetAllBuffModule());
        }
    }
}