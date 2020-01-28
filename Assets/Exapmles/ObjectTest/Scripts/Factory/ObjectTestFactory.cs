namespace Game.ObjectTest.Factory
{
    using GUnit = ECS.Unit.Unit;
    using ECS.Data;
    using ECS.Object;
    using ECS.Object.Data;
    using ECS.Object.Module;
    using ECS.Factory;
    using ECS;
    using ECS.Common;
    using Asset.Factory;
    using Game.ObjectTest.Data;
    using Game.ObjectTest.Module.Buff;
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

        static List<IObjectKeyboardControl> _keyboardControlModuleList = new List<IObjectKeyboardControl>();
        static void InitKeyboardControlModuleList()
        {
            _keyboardControlModuleList.Add(new DefaultObjectKeyboardControl());
        }

        public static List<IObjectKeyboardControl> GetAllKeyboardControlModule(this UnitFactory factory)
        {
            if (_keyboardControlModuleList.Count == 0)
            {
                InitKeyboardControlModuleList();
            }

            return _keyboardControlModuleList;
        }

        public static void CreatePlayer(this UnitFactory factory, GameObject gameObject)
        {
            var moduleMgr = WorldManager.Instance.Module;
            var requiredModuleGroup = moduleMgr.TagToModuleGroupType(ObjectConstant.OBJECT_MODULE_GROUP_NAME)
                | moduleMgr.TagToModuleGroupType(ObjectConstant.KEYBOARD_CONTROL_MODULE_GROUP_NAME)
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
            var controlProcessData = unit.AddData<ObjectKeyboardControlProcessData>();
            var controlDataList = controlProcessData.controlDataList;
            controlDataList.Add(Pool.Get<ObjectMoveLeftData>());
            controlDataList.Add(Pool.Get<ObjectMoveRightData>());
            controlDataList.Add(Pool.Get<ObjectFinishMoveLeftData>());
            controlDataList.Add(Pool.Get<ObjectFinishMoveRightData>());

            var factory = WorldManager.Instance.Factory;
            controlProcessData.allControlModuleList.AddRange(factory.GetAllKeyboardControlModule());
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