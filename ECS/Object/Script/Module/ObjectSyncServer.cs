namespace ECS.Object.Module
{
    using GUnit = ECS.Unit.Unit;
    using ECS.Module;
    using ECS.Data;
    using ECS.Object.Data;
    using System;
    using UniRx;
    using UnityEngine;

    public sealed class ObjectSyncServer : Module
    {
        public override int Group { get; protected set; } 
            = WorldManager.Instance.Module.TagToModuleGroupType(ObjectConstant.SYNC_MODULE_GROUP_NAME);

        public ObjectSyncServer()
        {
            RequiredDataList = new Type[]{
                typeof(ObjectSyncServerData)
            };
        }

        static ObjectSyncServerData _syncData;

        protected override void OnAdd(GUnit unit)
        {
            _syncData = unit.GetData<ObjectSyncServerData>();
            var systemData = unit.GetData<SystemData>();
            var unitData = unit.GetData<UnitData>();

            IDisposable preparedDispose = null;
            IDisposable updateDispose = null;
            _syncData.enable.Subscribe(_ => 
            {
                if (_)
                {
                    updateDispose = Observable.EveryUpdate().Subscribe(time =>
                    {
                        var currentInternalFrame = _syncData.internalFrame + 1;
                        if (currentInternalFrame >= _syncData.internalFrameSize)
                        {
                            currentInternalFrame = 0;
                            _syncData.currentKeyFrame = Mathf.Min(_syncData.currentKeyFrame++, _syncData.serverKeyFrame);
                        }

                        var currentFrame = _syncData.currentKeyFrame * _syncData.internalFrameSize + currentInternalFrame;
                        if (currentFrame > systemData.serverFrame)
                        {
                            systemData.serverFrame = currentFrame;
                            _syncData.syncSubject.OnNext(ValueTuple.Create(currentFrame, currentInternalFrame));

                            _syncData.internalFrame = currentInternalFrame;
                        }                        
                    });

                    preparedDispose = _syncData.preparedSyncInfoList.ObserveAdd().Subscribe(stateInfo =>
                    {
                        if (_syncData.nextFrameDispose != null)
                        {
                            return;
                        }

                        _syncData.nextFrameDispose = Observable.NextFrame().Subscribe(time =>
                        {
                            _syncData.nextFrameDispose.Dispose();
                            _syncData.nextFrameDispose = null;

                            SyncObjectState();
                        });
                    });
                }
                else
                {
                    updateDispose?.Dispose();
                    updateDispose = null;

                    preparedDispose?.Dispose();
                    preparedDispose = null;
                }
            }).AddTo(unitData.disposable);
        }

        public static void UpdateServerKeyFrame(int keyFrame)
        {
            if (_syncData.serverKeyFrame <= keyFrame)
                return;

            _syncData.serverKeyFrame = keyFrame;
        }

        public static IObservable<ValueTuple<int, int>> EverySyncUpdate()
        {
            return _syncData.syncSubject;
        }

        public static void AddState(GUnit unit, string stateName, Vector3 param, ObjectStateType stateType)
        {
            _syncData.preparedSyncInfoList.Add(new ServerSyncStateInfo()
            {
                unitId = unit.UnitId,
                stateName = stateName,
                stateParam = param,
                stateType = stateType
            });
        }


        static void SyncObjectState()
        {
            var preparedSyncInfoList = _syncData.preparedSyncInfoList;
            foreach (var serverSyncInfo in preparedSyncInfoList)
            {
                var unit = WorldManager.Instance.Unit.GetUnit(serverSyncInfo.unitId);
                ObjectSync.Add(unit, new SyncStateInfo()
                {
                    serverKeyFrame = _syncData.currentKeyFrame + 1,
                    internalFrame = _syncData.internalFrame,

                    stateName = serverSyncInfo.stateName,
                    stateParam = serverSyncInfo.stateParam,
                    stateType = serverSyncInfo.stateType
                });
            }

            preparedSyncInfoList.Clear();
        }
    }
}