namespace ECS.Module
{
    using GUnit = ECS.Unit.Unit;
    using ECS.Data;

    public interface IObjectBuff
    {
        string Name { get; }
        void Start(GUnit unit, ObjectBuffData buffData);
        void Stop(GUnit unit, ObjectBuffData buffData);
    }

    public abstract class ObjectBuff<T> : IObjectBuff where T : ObjectBuffData
    {
        public string Name { get { return typeof(T).ToString(); } }

        public void Start(GUnit unit, ObjectBuffData buffData)
        {
            OnStart(unit, buffData as T);
        }

        public void Stop(GUnit unit, ObjectBuffData buffData)
        {
            OnStop(unit, buffData as T);

            var processData = unit.GetData<ObjectBuffProcessData>();
            processData.currentBuffList.Remove(buffData);
            DataPool.Release(buffData);
        }

        protected abstract void OnStart(GUnit unit, T buffData);
        protected abstract void OnStop(GUnit unit, T buffData);
    }
}