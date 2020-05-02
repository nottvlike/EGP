namespace ECS.Module
{
    using GUnit = ECS.Unit.Unit;
    using ECS.Data;
    using ECS.Common;
    using UnityEngine;
    using System;

    public sealed class ObjectControlState : Module
    {
        public override int Group { get; protected set; }
            = WorldManager.Instance.Module.TagToModuleGroupType(ObjectConstant.CONTROL_MODULE_GROUP_NAME);

        public ObjectControlState()
        {
            RequiredDataList = new Type[]
            {
                typeof(ObjectControlStateData)
            };
        }

        protected override void OnAdd(GUnit unit)
        {
            var controlDataList = ObjectControlDataDict.Get(unit);
            foreach (var controlData in controlDataList)
            {
                var controlModule = ObjectControlModuleDict.Get(controlData.controlType);
                controlData.objectControl = controlModule;

                ObjectControlStateTypeDict.Set(unit, controlData.controlType, KeyStateType.None);
            }
        }

        protected override void OnRemove(GUnit unit)
        {
            var controlStateData = unit.GetData<ObjectControlStateData>();
            controlStateData.stateType.Value = ObjectControlStateType.Finish;

            var controlDataList = ObjectControlDataDict.Get(unit);
            foreach (var controlData in controlDataList)
            {
                Pool.Release(controlData);
            }
            ObjectControlDataDict.Clear(unit);
            ObjectControlStateTypeDict.Clear(unit);
        }

        public static void CheckAllControl(GUnit unit, int controlType, ObjectControlStateData controlStateData,
            ObjectStateProcessData stateProcessData)
        {
            var controlDataList = ObjectControlDataDict.Get(unit);
            foreach (var controlData in controlDataList)
            {
                if (!ContainsControlType(controlData.objectControl.ControlTypeList, controlType))
                {
                    continue;
                }

                var controlModule = controlData.objectControl;
                var result = controlModule.CheckControl(unit, controlData, stateProcessData);
                if (result.Item1)
                {
                    DoState(unit, controlData, result.Item2);
                }
            }
        }

        static bool ContainsControlType(int[] controlTypeList, int controlType)
        {
            foreach (var type in controlTypeList)
            {
                if (type == controlType)
                {
                    return true;
                }
            }

            return false;
        }

        static void DoState(GUnit unit, ObjectControlData controlData, Vector3 param)
        {
            if (controlData.stateType == ObjectStateType.Start)
            {
                ObjectStateProcess.Start(unit, controlData.stateId, param);
            }
            else if (controlData.stateType == ObjectStateType.Update)
            {
                ObjectStateProcess.Update(unit, controlData.stateId, param);
            }
            else if (controlData.stateType == ObjectStateType.Finish)
            {
                ObjectStateProcess.Finish(unit, controlData.stateId);
            }
        }
    }
}