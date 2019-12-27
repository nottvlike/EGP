namespace ECS.Unit
{
    using System;
    using System.Collections.Generic;
    using ECS.Data;
    using ECS.Module;

    public struct Unit
    {
        ModuleManager moduleMgr => WorldManager.Instance.Module;
        DataManager dataMgr => WorldManager.Instance.Data;

        public uint UnitId { get; private set; }

        public Unit(uint unitId)
        {
            UnitId = unitId;
        }

        public void AddData(IData data, Type key = null)
        {
            dataMgr.AddData(UnitId, data, key);
        }

        public void RemoveData(IData data)
        {
            dataMgr.RemoveData(UnitId, data);
        }

        public void RemoveData(Type type)
        {
            var data = GetData(type);
            RemoveData(data);
        }

        public IData GetData(Type type, bool includeDeleted = false)
        {
            return dataMgr.GetData(UnitId, type, includeDeleted);;
        }

        public T AddData<T>() where T : IData
        {
            var data = (T)DataPool.Get(typeof(T));
            AddData(data);
            return data;
        }

        public void RemoveData<T>() where T : IData
        {
            RemoveData(typeof(T));
        }

        public T GetData<T>() where T : IData
        {
            return (T)GetData(typeof(T));
        }

        public IEnumerable<IData> GetAllData(uint unitId)
        {
            return dataMgr.GetAllData(unitId);
        }

        public void UpdateMeetModuleList()
        {
            moduleMgr.UpdateMeetModuleList(this);            
        }
    }
}