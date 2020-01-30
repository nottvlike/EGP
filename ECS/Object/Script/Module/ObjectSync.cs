namespace ECS.Object.Module
{
    using GUnit = ECS.Unit.Unit;
    using ECS.Module;
    using ECS.Object.Data;
    using UniRx;
    using System;
    using ECS.Common;

    public sealed class ObjectSync : Module
    {
        public override int Group { get; protected set; } 
            = WorldManager.Instance.Module.TagToModuleGroupType(ObjectConstant.SYNC_MODULE_GROUP_NAME);

        public ObjectSync()
        {
            RequiredDataList = new Type[]{
                typeof(ObjectSyncData)
            };
        }

        protected override void OnAdd(GUnit unit)
        {
            var objectSyncData = unit.GetData<ObjectSyncData>();
            objectSyncData.stateSyncInfoList.ObserveAdd().Subscribe(_ => 
            {
                if (objectSyncData.checkSyncDispose != null)
                {
                    return;
                }

                objectSyncData.checkSyncDispose = ObjectSyncServer.EverySyncUpdate().Subscribe(frameInfo => 
                {
                    if (objectSyncData.stateSyncInfoList.Count == 0)
                    {
                        objectSyncData.checkSyncDispose.Dispose();
                        objectSyncData.checkSyncDispose = null;
                        return;
                    }

                    var stateSyncInfoList = objectSyncData.stateSyncInfoList;
                    for (var i = 0; i < stateSyncInfoList.Count;)
                    {
                        var syncInfo = objectSyncData.stateSyncInfoList[i];
#if DEBUG
                        if (syncInfo.serverKeyFrame < frameInfo.Item1
                            || (syncInfo.serverKeyFrame == frameInfo.Item1 && syncInfo.internalFrame < frameInfo.Item2))
                        {
                            Log.W("Sync {0} state {1} {2} failed!", syncInfo.stateType, 
                                syncInfo.stateName, syncInfo.stateParam);
                            objectSyncData.stateSyncInfoList.RemoveAt(i);
                            return;
                        }
#endif
                        if (syncInfo.serverKeyFrame == frameInfo.Item1 && syncInfo.internalFrame == frameInfo.Item2)
                        {
                            DoState(unit, syncInfo);
                            objectSyncData.stateSyncInfoList.RemoveAt(i);
                        }
                        else
                        {
                            i++;
                        }
                    }
                });
            });
        }

        void DoState(GUnit unit, SyncStateInfo syncInfo)
        {
            if (syncInfo.stateType == ObjectStateType.Start)
            {
                ObjectStateProcess.Start(unit, syncInfo.stateName, syncInfo.stateParam, false);
            }
            else if (syncInfo.stateType == ObjectStateType.Update)
            {
                ObjectStateProcess.Update(unit, syncInfo.stateName, syncInfo.stateParam, false);
            }
            else if (syncInfo.stateType == ObjectStateType.Finish)
            {
                ObjectStateProcess.Finish(unit, syncInfo.stateName, false);
            }
        }

        protected override void OnRemove(GUnit unit)
        {
            var objectSyncData = unit.GetData<ObjectSyncData>();
            objectSyncData.checkSyncDispose?.Dispose();
        }

        public static void AddState(GUnit unit, SyncStateInfo syncInfo)
        {
            var objectSyncData = unit.GetData<ObjectSyncData>();
            objectSyncData.stateSyncInfoList.Add(syncInfo);
        }
    }
}