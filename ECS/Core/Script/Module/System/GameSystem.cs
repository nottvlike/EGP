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
                typeof(SystemData),
                typeof(UpdateData),
                typeof(FixedUpdateData),
                typeof(LateUpdateData)
            };
        }

        static SystemData _systemData;
        static UpdateData _updateData;
        static FixedUpdateData _fixedUpdateData;
        static LateUpdateData _lateUpdateData;

        protected override void OnAdd(GUnit unit)
        {
            _systemData = unit.GetData<SystemData>();

            var unitData = unit.GetData<UnitData>();

            _updateData = unit.GetData<UpdateData>();
            _updateData.updateSubject = new Subject<int>();

            Observable.EveryUpdate().Subscribe(_ =>
            {
                var deltaTime = (int)(Time.deltaTime * Constant.SECOND_TO_MILLISECOND);
                _updateData.deltaTime = deltaTime;
                _systemData.time += deltaTime;
                _systemData.clientFrame++;

                _updateData.updateSubject.OnNext(deltaTime);
            }).AddTo(unitData.disposable);

            _lateUpdateData = unit.GetData<LateUpdateData>();
            _lateUpdateData.updateSubject = new Subject<int>();

            Observable.EveryLateUpdate().Subscribe(_ =>
            {
                _lateUpdateData.updateSubject.OnNext(_updateData.deltaTime);
            }).AddTo(unitData.disposable);

            _fixedUpdateData = unit.GetData<FixedUpdateData>();
            _fixedUpdateData.updateSubject = new Subject<int>();

            Observable.EveryFixedUpdate().Subscribe(_ =>
            {
                var deltaTime = (int)(Time.fixedDeltaTime * Constant.SECOND_TO_MILLISECOND);
                _fixedUpdateData.deltaTime = deltaTime;
                _fixedUpdateData.updateSubject.OnNext(_fixedUpdateData.deltaTime);
            }).AddTo(unitData.disposable);
        }

        protected override void OnRemove(GUnit unit)
        {
            base.OnRemove(unit);
            
            _systemData = null;

            _updateData.updateSubject.OnCompleted();
            _updateData = null;

            _lateUpdateData.updateSubject.OnCompleted();
            _lateUpdateData = null;

            _fixedUpdateData.updateSubject.OnCompleted();
            _fixedUpdateData = null;
        }

        public static IObservable<int> ObserveEveryUpdate()
        {
            return Observable.Defer(() =>
            {
                return _updateData.updateSubject;
            });
        }

        public static IObservable<int> ObserveEveryLateUpdate()
        {
            return Observable.Defer(() =>
            {
                return _lateUpdateData.updateSubject;
            });
        }

        public static IObservable<int> ObserveEveryFixedUpdate()
        {
            return Observable.Defer(() =>
            {
                return _fixedUpdateData.updateSubject;
            });
        }
    }
}