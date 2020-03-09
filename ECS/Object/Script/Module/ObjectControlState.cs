namespace ECS.Object.Module
{
    using GUnit = ECS.Unit.Unit;
    using ECS.Data;
    using ECS.Module;
    using ECS.Object.Data;
    using UnityEngine;
    using System;
    using UniRx;

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
            var controlStateData = unit.GetData<ObjectControlStateData>();
            foreach (var controlData in controlStateData.controlDataList)
            {
                foreach (var controlModule in controlStateData.controlModuleList)
                {
                    if (controlModule.ControlType == controlData.controlType)
                    {
                        controlData.objectControl = controlModule;
                    }
                }

                controlStateData.keyStateDict[controlData.controlType] = KeyStateType.None;
            }
        }

        protected override void OnRemove(GUnit unit)
        {
            var controlStateData = unit.GetData<ObjectControlStateData>();
            controlStateData.stateType.Value = ObjectControlStateType.Finish;
        }

        public static void CheckAllControl(GUnit unit, int controlType, ObjectControlStateData controlStateData,
            ObjectStateProcessData stateProcessData)
        {
            foreach (var controlData in controlStateData.controlDataList)
            {
                if (!ContainsControlType(controlData.objectControl.ControlTypeList, controlType))
                {
                    continue;
                }

                var controlModule = controlData.objectControl;
                var result = controlModule.CheckControl(controlData, controlStateData, stateProcessData);
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