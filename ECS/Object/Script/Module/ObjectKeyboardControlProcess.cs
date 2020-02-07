namespace ECS.Object.Module
{
    using GUnit = ECS.Unit.Unit;
    using ECS.Module;
    using ECS.Data;
    using ECS.Object.Data;
    using UniRx;
    using UnityEngine;
    using System;

    public sealed class ObjectKeyboardControlProcess : Module
    {
        public override int Group { get; protected set; } 
            = WorldManager.Instance.Module.TagToModuleGroupType(ObjectConstant.KEYBOARD_CONTROL_MODULE_GROUP_NAME);

        public ObjectKeyboardControlProcess()
        {
            RequiredDataList = new Type[]
            {
                typeof(ObjectKeyboardControlProcessData),
                typeof(ObjectStateProcessData)
            };
        }

        protected override void OnAdd(GUnit unit)
        {
            var unitData = unit.GetData<UnitData>();
            var processData = unit.GetData<ObjectKeyboardControlProcessData>();
            var stateProcerocessData = unit.GetData<ObjectStateProcessData>();
            Observable.EveryUpdate().Subscribe(_ => 
            {
                foreach (var controlModule in processData.allControlModuleList)
                {
                    foreach (var controlData in processData.controlDataList)
                    {
                        if (controlModule.ControlType == controlData.controlType)
                        {
                            var result = controlModule.CheckControl(controlData, stateProcerocessData);
                            if (result.Item1)
                            {
                                DoState(unit, controlData, result.Item2);
                            }
                        }
                    }
                }

            }).AddTo(unitData.disposable);
        }

        void DoState(GUnit unit, IObjectKeyboardControlData controlData, Vector3 param)
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