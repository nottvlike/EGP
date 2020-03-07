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
            = WorldManager.Instance.Module.TagToModuleGroupType(ObjectConstant.CONTROL_MODULE_GROUP_NAME);

        public ObjectUIControlProcess()
        {
            RequiredDataList = new Type[]
            {
                typeof(ObjectControlStateData),
                typeof(ObjectUIControlProcessData),
                typeof(ObjectStateProcessData)
            };
        }

        protected override void OnAdd(GUnit unit)
        {
            var unitData = unit.GetData<UnitData>();
            var controlStateData = unit.GetData<ObjectControlStateData>();
            var controlProcessData = unit.GetData<ObjectUIControlProcessData>();
            var stateProcerocessData = unit.GetData<ObjectStateProcessData>();

            foreach (var controlData in controlProcessData.controlDataList)
            {
                controlData.controlHelper.ObservePointerDown().Subscribe(_ =>
                {
                    controlStateData.state[controlData.controlType] = ControlStateType.Down;
                    ObjectControlState.CheckAllControl(unit, controlData.controlType, controlStateData,
                        stateProcerocessData);
                }).AddTo(unitData.disposable);
                controlData.controlHelper.ObservePointerUp().Subscribe(_ =>
                {
                    controlStateData.state[controlData.controlType] = ControlStateType.Up;
                    ObjectControlState.CheckAllControl(unit, controlData.controlType, controlStateData,
                        stateProcerocessData);
                    controlStateData.state[controlData.controlType] = ControlStateType.None;
                }).AddTo(unitData.disposable);
            }
        }
    }
}