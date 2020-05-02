namespace ECS.Module
{
    using GUnit = ECS.Unit.Unit;
    using ECS.Data;
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
            var stateProcessData = unit.GetData<ObjectStateProcessData>();

            controlStateData.stateType.Subscribe(controlStateType =>
            {
                if (controlStateType == ObjectControlStateType.Start)
                {
                    controlProcessData.checkDispose = GameSystem.ObserveEveryUpdate().Subscribe(_ =>
                    {
                        foreach (var controlData in controlProcessData.controlDataList)
                        {
                            if (Input.GetKeyDown(controlData.key))
                            {
                                ObjectControlStateTypeDict.Set(unit, controlData.controlType, KeyStateType.Down);
                                ObjectControlState.CheckAllControl(unit, controlData.controlType, controlStateData,
                                    stateProcessData);
                            }
                            else if (Input.GetKeyUp(controlData.key))
                            {
                                ObjectControlStateTypeDict.Set(unit, controlData.controlType, KeyStateType.Up);
                                ObjectControlState.CheckAllControl(unit, controlData.controlType, controlStateData,
                                    stateProcessData);
                                ObjectControlStateTypeDict.Set(unit, controlData.controlType, KeyStateType.None);
                            }
                        }
                    });
                }
                else if (controlStateType == ObjectControlStateType.Finish)
                {
                    controlProcessData.checkDispose?.Dispose();
                }
            }).AddTo(unitData.disposable);
        }
    }
}