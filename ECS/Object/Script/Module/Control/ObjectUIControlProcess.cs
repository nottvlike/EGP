namespace ECS.Module
{
    using GUnit = ECS.Unit.Unit;
    using ECS.Data;
    using UniRx;
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
            var stateProcessData = unit.GetData<ObjectStateProcessData>();

            controlStateData.stateType.Subscribe(controlStateType =>
            {
                if (controlStateType == ObjectControlStateType.Start)
                {
                    controlProcessData.checkDispose = new CompositeDisposable();
                    foreach (var controlData in controlProcessData.controlDataList)
                    {
                        controlData.controlHelper.ObservePointerDown().Subscribe(_ =>
                        {
                            ObjectControlStateTypeDict.Set(unit, controlData.controlType, KeyStateType.Down);
                            ObjectControlState.CheckAllControl(unit, controlData.controlType, controlStateData,
                                stateProcessData);
                        }).AddTo(controlProcessData.checkDispose);

                        controlData.controlHelper.ObservePointerUp().Subscribe(_ =>
                        {
                            ObjectControlStateTypeDict.Set(unit, controlData.controlType, KeyStateType.Up);
                            ObjectControlState.CheckAllControl(unit, controlData.controlType, controlStateData,
                                stateProcessData);
                            ObjectControlStateTypeDict.Set(unit, controlData.controlType, KeyStateType.None);
                        }).AddTo(controlProcessData.checkDispose);
                    }
                }
                else if (controlStateType == ObjectControlStateType.Finish)
                {
                    controlProcessData.checkDispose?.Dispose();
                }
            }).AddTo(unitData.disposable);
        }
    }
}