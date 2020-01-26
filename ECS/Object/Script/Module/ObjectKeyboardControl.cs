namespace ECS.Object.Module
{
    using GUnit = ECS.Unit.Unit;
    using ECS.Module;
    using ECS.Data;
    using ECS.Object.Data;
    using UniRx;
    using UnityEngine;
    using System;

    public abstract class ObjectKeyboardControl<T> : Module where T : ObjectKeyboardControlData
    {
        public override int Group { get; protected set; } 
            = WorldManager.Instance.Module.TagToModuleGroupType(ObjectConstant.KEYBOARD_CONTROL_MODULE_GROUP_NAME);

        public ObjectKeyboardControl()
        {
            RequiredDataList = new Type[]
            {
                typeof(T)
            };
        }

        protected override void OnAdd(GUnit unit)
        {
            var controlData = GetControlData(unit);
            var unitData = unit.GetData<UnitData>();
            Observable.EveryUpdate().Subscribe(_ => 
            {
                if (controlData.mouseButton == -1)
                {
                    if (controlData.controlStateType == ControlStateType.Down 
                        && Input.GetKeyDown(controlData.key))
                    {
                        DoState(unit, controlData);
                    }
                    else if (controlData.controlStateType == ControlStateType.Up 
                        && Input.GetKeyUp(controlData.key))
                    {
                        DoState(unit, controlData);
                    }
                }
                else
                {
                    if (controlData.controlStateType == ControlStateType.Down 
                        && Input.GetMouseButtonDown(controlData.mouseButton))
                    {
                        DoState(unit, controlData);
                    }
                    else if (controlData.controlStateType == ControlStateType.Up
                        && Input.GetMouseButtonUp(controlData.mouseButton))
                    {
                        DoState(unit, controlData);
                    }
                }
            }).AddTo(unitData.disposable);
        }

        void DoState(GUnit unit, ObjectControlData controlData)
        {
            if (controlData.stateType == ObjectStateType.Start)
            {
                ObjectStateProcess.Start(unit, controlData.stateName, GetStateParam(unit));
            }
            else if (controlData.stateType == ObjectStateType.Finish)
            {
                ObjectStateProcess.Finish(unit, controlData.stateName);
            }
        }

        protected ObjectKeyboardControlData GetControlData(GUnit unit)
        {
            return unit.GetData<T>();
        }

        protected abstract Vector3 GetStateParam(GUnit unit);
    }
}