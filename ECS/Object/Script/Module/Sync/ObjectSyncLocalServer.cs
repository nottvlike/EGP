namespace ECS.Module
{
    using GUnit = ECS.Unit.Unit;
    using ECS;
    using ECS.Data;
    using System;
    using UniRx;

    public sealed class ObjectSyncLocalServer : SingleModule
    {
        public override int Group { get; protected set; } 
            = WorldManager.Instance.Module.TagToModuleGroupType(ObjectConstant.SYNC_MODULE_GROUP_NAME);

        public ObjectSyncLocalServer()
        {
            RequiredDataList = new Type[]{
                typeof(ObjectSyncServerData)
            };
        }

        ObjectSyncServerData _syncData;
        protected override void OnAdd(GUnit unit)
        {
            var unitData = unit.GetData<UnitData>();
            _syncData = unit.GetData<ObjectSyncServerData>();
            IDisposable updateServerKeyFrameDispose = null;
            _syncData.enable.Subscribe(_ => 
            {
                if (_)
                {
                    updateServerKeyFrameDispose = Observable.Interval(TimeSpan.FromMilliseconds(100)).Subscribe(time =>
                    {
                        _syncData.serverKeyFrame++;
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
            _syncData.enable.Value = false;
            _syncData = null;
        }
    }
}