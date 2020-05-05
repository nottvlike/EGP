namespace ECS.Module
{
    using GUnit = ECS.Unit.Unit;
    using ECS.Data;

    public interface IObjectBuff
    {
        int Id { get; }
        void Start(GUnit unit, IBuffData buffData, bool removeWhenFinish);
        void Stop(GUnit unit, IBuffData buffData);
        void Finish(GUnit unit, IBuffData buffData, bool removeFromBuffDataList);
    }

    public abstract class ObjectBuff<T> : IObjectBuff where T: class, IBuffData
    {
        public int Id { get; } = typeof(T).GetHashCode();

        public void Start(GUnit unit, IBuffData buffData, bool removeWhenFinish)
        {
            var processData = unit.GetData<ObjectBuffProcessData>();
            if (processData.currentBuffDataList.IndexOf(buffData) == -1)
            {
                processData.currentBuffDataList.Add(buffData);
                OnStart(unit, buffData as T, removeWhenFinish);
            }
            else
            {
                OnUpdate(unit, buffData as T);
            }
        }

        public void Stop(GUnit unit, IBuffData buffData)
        {
            OnStop(unit, buffData as T);
        }

        public void Finish(GUnit unit, IBuffData buffData, bool removeFromBuffDataList) 
        {
            OnFinish(unit, buffData as T);

            if (removeFromBuffDataList)
            {
                var processData = unit.GetData<ObjectBuffProcessData>();
                processData.currentBuffDataList.Remove(buffData);
            }
            DataPool.Release(buffData);
        }

        protected virtual void OnStart(GUnit unit, T buffData, bool removeWhenFinish) {}
        protected virtual void OnUpdate(GUnit unit, T buffData) {}
        protected virtual void OnStop(GUnit unit, T buffData) {}
        protected virtual void OnFinish(GUnit unit, T buffData) {}
    }
}