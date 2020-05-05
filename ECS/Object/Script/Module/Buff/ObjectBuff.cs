namespace ECS.Module
{
    using GUnit = ECS.Unit.Unit;
    using ECS.Data;

    public abstract class ObjectBuff
    {
        public virtual int Id { get; }

        public void Start(GUnit unit, ObjectBuffProcessData processData, IBuffData buffData)
        {
            processData.currentBuffDataList.Add(buffData);

            OnStart(unit, buffData);
        }

        public void Update(GUnit unit, ObjectBuffProcessData processData, IBuffData buffData)
        {
            OnUpdate(unit, buffData);
        }

        public void Stop(GUnit unit, ObjectBuffProcessData processData, IBuffData buffData)
        {
            OnStop(unit, buffData);
        }

        public void Finish(GUnit unit, ObjectBuffProcessData processData, IBuffData buffData) 
        {
            OnFinish(unit, buffData);

            processData.currentBuffDataList.Remove(buffData);
            DataPool.Release(buffData);
        }

        protected void OnStart(GUnit unit, IBuffData buffData) {}
        protected void OnUpdate(GUnit unit, IBuffData buffData) {}
        protected void OnStop(GUnit unit, IBuffData buffData) {}
        protected void OnFinish(GUnit unit, IBuffData buffData) {}
    }
}