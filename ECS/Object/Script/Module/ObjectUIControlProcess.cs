namespace ECS.Object.Module
{
    using GUnit = ECS.Unit.Unit;
    using ECS.Module;
    using ECS.Data;
    using ECS.Object.Data;
    using UniRx;
    using UnityEngine;
    using System;

    public sealed class ObjectUIControlProcess : Module
    {
        public override int Group { get; protected set; }
            = WorldManager.Instance.Module.TagToModuleGroupType(ObjectConstant.UI_CONTROL_MODULE_GROUP_NAME);

        public ObjectUIControlProcess()
        {
            RequiredDataList = new Type[]
            {
                typeof(ObjectUIControlProcessData),
                typeof(ObjectStateProcessData)
            };
        }

        protected override void OnAdd(GUnit unit)
        {
            var unitData = unit.GetData<UnitData>();
            var processData = unit.GetData<ObjectUIControlProcessData>();
            var stateProcerocessData = unit.GetData<ObjectStateProcessData>();

            foreach (var controlData in processData.allControlDataList)
            {
                foreach (var controlModule in processData.allControlModuleList)
                {
                    if (controlModule.ControlDataType == controlData.GetType())
                    {
                        controlModule.Bind(controlData);
                    }
                }

                processData.currentState[controlData.controlType] = ControlStateType.None;

                controlData.controlHelper.ObservePointerDown().Subscribe(_ =>
                {
                    processData.currentState[controlData.controlType] = ControlStateType.Down;
                    CheckAllUControl(unit, controlData.controlType, processData, stateProcerocessData);
                }).AddTo(unitData.disposable);
                controlData.controlHelper.ObservePointerUp().Subscribe(_ =>
                {
                    processData.currentState[controlData.controlType] = ControlStateType.Up;
                    CheckAllUControl(unit, controlData.controlType, processData, stateProcerocessData);
                    processData.currentState[controlData.controlType] = ControlStateType.None;
                }).AddTo(unitData.disposable);
            }
        }

        void CheckAllUControl(GUnit unit, int controlType, ObjectUIControlProcessData processData,
            ObjectStateProcessData stateProcessData)
        {
            foreach (var controlModule in processData.allControlModuleList)
            {
                if (controlModule.ControlTypeList.IndexOf(controlType) == -1)
                {
                    continue;
                }

                var result = controlModule.CheckControl(processData, stateProcessData);
                if (result.Item1)
                {
                    DoState(unit, controlModule.ControlData, result.Item2);
                }
            }
        }

        void DoState(GUnit unit, ObjectUIControlData controlData, Vector3 param)
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