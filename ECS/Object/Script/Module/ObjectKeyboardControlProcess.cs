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
            = WorldManager.Instance.Module.TagToModuleGroupType(ObjectConstant.CONTROL_MODULE_GROUP_NAME);

        public ObjectKeyboardControlProcess()
        {
            RequiredDataList = new Type[]
            {
                typeof(ObjectControlStateData),
                typeof(ObjectKeyboardControlProcessData),
                typeof(ObjectStateProcessData)
            };
        }

        protected override void OnAdd(GUnit unit)
        {
            var unitData = unit.GetData<UnitData>();
            var controlStateData = unit.GetData<ObjectControlStateData>();
            var controlProcessData = unit.GetData<ObjectKeyboardControlProcessData>();
            var stateProcerocessData = unit.GetData<ObjectStateProcessData>();
            GameSystem.ObserveEveryUpdate().Subscribe(_ => 
            {
                foreach (var controlData in controlProcessData.controlDataList)
                {
                    if (Input.GetKeyDown(controlData.key))
                    {
                        controlStateData.state[controlData.controlType] = ControlStateType.Down;
                        ObjectControlState.CheckAllControl(unit, controlData.controlType, controlStateData,
                            stateProcerocessData);
                    }
                    else if (Input.GetKeyUp(controlData.key))
                    {
                        controlStateData.state[controlData.controlType] = ControlStateType.Up;
                        ObjectControlState.CheckAllControl(unit, controlData.controlType, controlStateData,
                            stateProcerocessData);
                        controlStateData.state[controlData.controlType] = ControlStateType.None;
                    }
                }

            }).AddTo(unitData.disposable);
        }
    }
}