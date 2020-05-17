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

        public void RemoveData(IData data, bool realRemoveWhenDestroy = true)
        {
            dataMgr.RemoveData(UnitId, data, realRemoveWhenDestroy);
        }

        public void RemoveData(Type type, bool realRemoveWhenDestroy = true)
        {
            var data = GetData(type);
            RemoveData(data, realRemoveWhenDestroy);
        }

        public IData GetData(Type type, bool includeDeleted = true)
        {
            return dataMgr.GetData(UnitId, type, includeDeleted);;
        }

        public IData TryGetData(Type type, bool includeDeleted = true)
        {
            return dataMgr.TryGetData(UnitId, type, includeDeleted);;
        }

        public T AddData<T>() where T : IData
        {
            var data = (T)DataPool.Get(typeof(T));
            AddData(data);
            return data;
        }

        public void RemoveData<T>(bool realRemoveWhenDestroy = true) where T : IData
        {
            RemoveData(typeof(T), realRemoveWhenDestroy);
        }

        public T GetData<T>(bool includeDeleted = true) where T : IData
        {
            return (T)GetData(typeof(T), includeDeleted);
        }

        public T TryGetData<T>(bool includeDeleted = true) where T : IData
        {
            return (T)TryGetData(typeof(T), includeDeleted);
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