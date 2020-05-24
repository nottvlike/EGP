namespace ECS.Unit
{
    using System;
    using System.Collections.Generic;
    using ECS.Data;
    using ECS.Module;

    public struct Unit
    {
        UnitManager unitMgr => WorldManager.Instance.Unit;
        ModuleManager moduleMgr => WorldManager.Instance.Module;
        DataManager dataMgr => WorldManager.Instance.Data;

        public uint UnitId { get; private set; }

        public int RequiredModuleGroup { get; private set; }

        public Unit(uint unitId, int requiredModuleGroup)
        {
            UnitId = unitId;
            RequiredModuleGroup = requiredModuleGroup;
        }

        public void AddData(IData data, Type key = null)
        {
            dataMgr.AddData(UnitId, data, key);
        }

        public void RemoveData(IData data, bool cacheData = false)
        {
            dataMgr.RemoveData(UnitId, data, cacheData);
        }

        public void RemoveData(Type type, bool cacheData = false)
        {
            var data = GetData(type);
            RemoveData(data, cacheData);
        }

        public IData GetData(Type type, bool includeCacheData = false)
        {
            return dataMgr.GetData(UnitId, type, includeCacheData);;
        }

        public IData TryGetData(Type type, bool includeCacheData = false)
        {
            return dataMgr.TryGetData(UnitId, type, includeCacheData);;
        }

        public T AddData<T>() where T : IData
        {
            var data = (T)DataPool.Get(typeof(T));
            AddData(data);
            return data;
        }

        public void RemoveData<T>(bool cacheData = false) where T : IData
        {
            RemoveData(typeof(T), cacheData);
        }

        public T GetData<T>(bool includeCacheData = false) where T : IData
        {
            return (T)GetData(typeof(T), includeCacheData);
        }

        public T TryGetData<T>(bool includeCacheData = false) where T : IData
        {
            return (T)TryGetData(typeof(T), includeCacheData);
        }

        public IEnumerable<IData> GetAllData()
        {
            return dataMgr.GetAllData(UnitId);
        }

        public void UpdateRequiredModuleGroup(int group)
        {
            RequiredModuleGroup |= group;
            unitMgr.AddUnit(UnitId, this, true);
        }

        public void UpdateMeetModuleList()
        {
            moduleMgr.UpdateMeetModuleList(this);            
        }
    }
}