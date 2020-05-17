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

        bool IsAddedBuff(ObjectBuffProcessData processData, IBuffData buffData)
        {
            if (processData.currentBuffDataList.IndexOf(buffData) != -1)
            {
                return true;
            }

            foreach (var addedBuffData in processData.currentBuffDataList)
            {
                if (addedBuffData.GetType() == buffData.GetType())
                {
                    return true;
                }
            }

            return false;
        }

        public void Start(GUnit unit, IBuffData buffData, bool removeWhenFinish)
        {
            var processData = unit.GetData<ObjectBuffProcessData>();
            if (!IsAddedBuff(processData, buffData))
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

        public void Finish(GUnit unit, IBuffData buffData, bool removeFromBuffDataList = true) 
        {
            var finishResult = OnFinish(unit, buffData as T);
            if (finishResult && removeFromBuffDataList)
            {
                var processData = unit.GetData<ObjectBuffProcessData>();
                for (var i = 0; i < processData.currentBuffDataList.Count; i++)
                {
                    var addedBuffData = processData.currentBuffDataList[i];
                    if (addedBuffData.GetType() == buffData.GetType())
                    {
                        processData.currentBuffDataList.Remove(addedBuffData);
                        break;
                    }
                }
            }
        }

        protected virtual void OnStart(GUnit unit, T buffData, bool removeWhenFinish) {}
        protected virtual void OnUpdate(GUnit unit, T buffData) {}
        protected virtual void OnStop(GUnit unit, T buffData) {}
        protected virtual bool OnFinish(GUnit unit, T buffData) { return true; }
    }
}