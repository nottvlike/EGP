namespace ECS.Module
{
    using Data;
    using System;
    using GUnit = ECS.Unit.Unit;
    using UniRx;
    using UnityEngine;

    public sealed class GameSystem : SingleModule
    {
        public override int Group { get; protected set; } 
            = WorldManager.Instance.Module.TagToModuleGroupType(Constant.SYSTEM_MODULE_GROUP_NAME);

        public GameSystem()
        {
            RequiredDataList = new Type[]{
                typeof(SystemData)
            };
        }

        static SystemData _systemData;
        protected override void OnAdd(GUnit unit)
        {
            _systemData = unit.GetData<SystemData>();
            _systemData.updateSubject = new Subject<int>();

            var unitData = unit.GetData<UnitData>();

            Observable.EveryUpdate().Subscribe(_ =>
            {
                var deltaTime = (int)(Time.deltaTime * Constant.SECOND_TO_MILLISECOND);
                _systemData.deltaTime = deltaTime;
                _systemData.time += deltaTime;
                _systemData.clientFrame++;

                _systemData.updateSubject.OnNext(deltaTime);
            }).AddTo(unitData.disposable);
        }

        protected override void OnRemove(GUnit unit)
        {
            base.OnRemove(unit);

            _systemData.updateSubject.OnCompleted();
            _systemData = null;
        }

        public static IObservable<int> ObserveEveryUpdate()
        {
            return Observable.Defer(() =>
            {
                return _systemData.updateSubject;
            });
        }
    }
}