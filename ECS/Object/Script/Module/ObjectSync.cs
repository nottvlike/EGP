namespace ECS.Object.Module
{
    using GUnit = ECS.Unit.Unit;
    using ECS.Module;
    using ECS.Object.Data;
    using UniRx;
    using System;

    public sealed class ObjectSync : Module
    {
        public override int Group { get; protected set; } 
            = WorldManager.Instance.Module.TagToModuleGroupType(ObjectConstant.OBJECT_MODULE_GROUP_NAME);

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

                    var syncInfo = objectSyncData.stateSyncInfoList[0];
                    if (syncInfo.serverKeyFrame == frameInfo.Item1 && syncInfo.internalFrame == frameInfo.Item2)
                    {
                        ObjectStateProcess.Start(unit, syncInfo.stateName, syncInfo.stateParam);
                        objectSyncData.stateSyncInfoList.RemoveAt(0);
                    }
                });
            });
        }

        protected override void OnRemove(GUnit unit)
        {
            var objectSyncData = unit.GetData<ObjectSyncData>();
            objectSyncData.checkSyncDispose?.Dispose();
        }

        public static void Add(GUnit unit, SyncStateInfo syncInfo)
        {
            var objectSyncData = unit.GetData<ObjectSyncData>();
            objectSyncData.stateSyncInfoList.Add(syncInfo);
        }
    }
}