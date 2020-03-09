namespace ECS.Object.Module
{
    using GUnit = ECS.Unit.Unit;
    using ECS.Module;
    using ECS.Data;
    using ECS.Object.Data;
    using System;
    using System.Linq;
    using UniRx;
    using UnityEngine;

    public sealed class ObjectSyncServer : SingleModule
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
                    updateDispose = GameSystem.ObserveEveryUpdate().Subscribe(time =>
                    {
                        var currentInternalFrame = _syncData.internalFrame + 1;
                        if (currentInternalFrame >= _syncData.internalFrameSize)
                        {
                            currentInternalFrame = 0;
                            _syncData.currentKeyFrame += _syncData.currentKeyFrame > _syncData.serverKeyFrame ? 0 : 1;
                        }

                        var currentFrame = _syncData.currentKeyFrame * _syncData.internalFrameSize + currentInternalFrame;
                        if (currentFrame > systemData.serverFrame)
                        {
                            systemData.serverFrame = currentFrame;
                            _syncData.syncSubject.OnNext(ValueTuple.Create(_syncData.currentKeyFrame, currentInternalFrame));

                            _syncData.internalFrame = currentInternalFrame;
                        }                        
                    });

                    preparedDispose = _syncData.preparedSyncInfoList.ObserveAdd().Subscribe(stateInfo =>
                    {
                        SyncObjectState();
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

        protected override void OnRemove(GUnit unit)
        {
            _syncData.enable.Value = false;
            _syncData = null;
        }

        public static IObservable<ValueTuple<int, int>> EverySyncUpdate()
        {
            return _syncData.syncSubject;
        }

        public static bool CanAddState(GUnit unit, int stateId, Vector3 param, ObjectStateType stateType)
        {
            var cachedSyncInfoList = _syncData.cachedSyncInfoList;
            var currentKeyFrame = _syncData.currentKeyFrame;
            for (var i = 0; i < cachedSyncInfoList.Count;)
            {
                var syncStateInfo = cachedSyncInfoList[i];
                if (syncStateInfo.serverKeyFrame < currentKeyFrame
                    && currentKeyFrame - syncStateInfo.serverKeyFrame > 2)
                {
                    cachedSyncInfoList.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }

            var hasSame = false;
            var sameStateList = cachedSyncInfoList.Where(_ => _.stateId == stateId).ToList();
            for (var i = 0; i < sameStateList.Count; i++)
            {
                var state = sameStateList[i];
                if (!hasSame && state.stateType == stateType && state.stateParam == param)
                {
                    hasSame = true;
                }
                else if (hasSame && (state.stateType != stateType || state.stateParam != param))
                {
                    hasSame = false;
                }
            }

            return !hasSame;
        }

        public static void AddState(GUnit unit, int stateId, Vector3 param, ObjectStateType stateType)
        {
            if (!CanAddState(unit, stateId, param, stateType))
            {
                return;
            }

            var syncStateInfo = new ServerSyncStateInfo()
            {
                serverKeyFrame = _syncData.currentKeyFrame,
                unitId = unit.UnitId,
                stateId = stateId,
                stateParam = param,
                stateType = stateType
            };

            _syncData.cachedSyncInfoList.Add(syncStateInfo);
            _syncData.preparedSyncInfoList.Add(syncStateInfo);
        }

        static void SyncObjectState()
        {
            var offsetKeyFrame = 5;
            var preparedSyncInfoList = _syncData.preparedSyncInfoList;
            foreach (var serverSyncInfo in preparedSyncInfoList)
            {
                var syncStateInfo = new SyncStateInfo()
                {
                    stateId = serverSyncInfo.stateId,
                    stateParam = serverSyncInfo.stateParam,
                    stateType = serverSyncInfo.stateType
                };

                var totalOffset = _syncData.internalFrame + offsetKeyFrame;
                syncStateInfo.serverKeyFrame = _syncData.currentKeyFrame + totalOffset / _syncData.internalFrameSize;
                syncStateInfo.internalFrame = totalOffset % _syncData.internalFrameSize;

                var unit = WorldManager.Instance.Unit.GetUnit(serverSyncInfo.unitId);
                ObjectSync.AddState(unit, syncStateInfo);
            }

            preparedSyncInfoList.Clear();
        }
    }
}