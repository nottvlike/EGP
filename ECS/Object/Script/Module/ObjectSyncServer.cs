namespace ECS.Object.Module
{
    using GUnit = ECS.Unit.Unit;
    using ECS.Module;
    using ECS.Data;
    using ECS.Object.Data;
    using System;
    using System.Linq;
    using System.Collections.Generic;
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
                            _syncData.currentKeyFrame = Mathf.Min(++_syncData.currentKeyFrame, _syncData.serverKeyFrame);
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

        public static bool CanAddState(GUnit unit, string stateName, Vector3 param, ObjectStateType stateType)
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
            var sameStateList = cachedSyncInfoList.Where(_ => _.stateName == stateName).ToList();
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

        public static void AddState(GUnit unit, string stateName, Vector3 param, ObjectStateType stateType)
        {
            if (!CanAddState(unit, stateName, param, stateType))
            {
                return;
            }

            var syncStateInfo = new ServerSyncStateInfo()
            {
                serverKeyFrame = _syncData.currentKeyFrame,
                unitId = unit.UnitId,
                stateName = stateName,
                stateParam = param,
                stateType = stateType
            };

            _syncData.cachedSyncInfoList.Add(syncStateInfo);
            _syncData.preparedSyncInfoList.Add(syncStateInfo);
        }


        public static bool CanIncreaseInternalFrame(int current, int delta = 1)
        {
            return current + delta < _syncData.internalFrameSize;
        }

        static List<SyncStateCountInfo> unitSyncList = new List<SyncStateCountInfo>();
        static int Increase(uint unitId)
        {
            for (var i = 0; i < unitSyncList.Count; i++)
            {
                var unitSync = unitSyncList[i];
                if (unitSync.unitId == unitId)
                {
                    if (unitSync.serverKeyFrame == _syncData.currentKeyFrame 
                        && unitSync.internalFrame == _syncData.internalFrame)
                    {
                        unitSync.count += 1;
                    }
                    else
                    {
                        unitSync.serverKeyFrame = _syncData.currentKeyFrame;
                        unitSync.internalFrame = _syncData.internalFrame;
                        unitSync.count = 1;
                    }

                    unitSyncList[i] = unitSync;
                    return unitSync.count;
                }
            }

            var syncCountInfo = new SyncStateCountInfo()
            {
                serverKeyFrame = _syncData.currentKeyFrame,
                internalFrame = _syncData.internalFrame,
                unitId = unitId,
                count = 1
            };
            unitSyncList.Add(syncCountInfo);
            return syncCountInfo.count;
        }

        static void SyncObjectState()
        {
            var offsetKeyFrame = 5;
            var preparedSyncInfoList = _syncData.preparedSyncInfoList;
            foreach (var serverSyncInfo in preparedSyncInfoList)
            {
                var syncStateInfo = new SyncStateInfo()
                {
                    stateName = serverSyncInfo.stateName,
                    stateParam = serverSyncInfo.stateParam,
                    stateType = serverSyncInfo.stateType
                };

                var offset = Increase(serverSyncInfo.unitId);
                var totalOffset = _syncData.internalFrame + offsetKeyFrame + offset;
                syncStateInfo.serverKeyFrame = _syncData.currentKeyFrame + totalOffset / _syncData.internalFrameSize;
                syncStateInfo.internalFrame = totalOffset % _syncData.internalFrameSize;

                var unit = WorldManager.Instance.Unit.GetUnit(serverSyncInfo.unitId);
                ObjectSync.Add(unit, syncStateInfo);
            }

            preparedSyncInfoList.Clear();
        }
    }
}