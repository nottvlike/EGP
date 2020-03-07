namespace ECS.Object.Module
{
    using GUnit = ECS.Unit.Unit;
    using ECS.Module;
    using ECS.Object.Data;
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
            var controlStateData = unit.GetData<ObjectControlStateData>();
            foreach (var controlData in controlStateData.controlDataList)
            {
                foreach (var controlModule in controlStateData.controlModuleList)
                {
                    if (controlModule.ControlType == controlData.controlType)
                    {
                        controlModule.Bind(controlData);
                    }
                }

                controlStateData.state[controlData.controlType] = ControlStateType.None;
            }
        }

        public static void CheckAllControl(GUnit unit, int controlType, ObjectControlStateData controlStateData,
            ObjectStateProcessData stateProcessData)
        {
            foreach (var controlModule in controlStateData.controlModuleList)
            {
                if (controlModule.ControlTypeList.IndexOf(controlType) == -1)
                {
                    continue;
                }

                var result = controlModule.CheckControl(controlStateData, stateProcessData);
                if (result.Item1)
                {
                    DoState(unit, controlModule.ControlData, result.Item2);
                }
            }
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