namespace ECS.Object.Module
{
    using GUnit = ECS.Unit.Unit;
    using ECS;
    using ECS.Data;
    using ECS.Module;
    using ECS.Object;
    using ECS.Object.Data;
    using System;
    using UniRx;

    public sealed class ObjectSyncLocalServer : Module
    {
        public override int Group { get; protected set; } 
            = WorldManager.Instance.Module.TagToModuleGroupType(ObjectConstant.SYNC_MODULE_GROUP_NAME);

        public ObjectSyncLocalServer()
        {
            RequiredDataList = new Type[]{
                typeof(ObjectSyncServerData)
            };
        }
        
        protected override void OnAdd(GUnit unit)
        {
            var unitData = unit.GetData<UnitData>();
            var syncData = unit.GetData<ObjectSyncServerData>();
            IDisposable updateServerKeyFrameDispose = null;
            syncData.enable.Subscribe(_ => 
            {
                if (_)
                {
                    updateServerKeyFrameDispose = Observable.Interval(TimeSpan.FromMilliseconds(100)).Subscribe(time =>
                    {
                        syncData.serverKeyFrame++;
                    });
                }
                else
                {
                    updateServerKeyFrameDispose?.Dispose();
                    updateServerKeyFrameDispose = null;
                }
            }).AddTo(unitData.disposable);
        }

        protected override void OnRemove(GUnit unit)
        {
            var syncData = unit.GetData<ObjectSyncServerData>();
            syncData.enable.Value = false;
        }
    }
}