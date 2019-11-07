namespace ECS.Entity
{
    using System;
    using System.Collections.Generic;
    using UniRx;
    using Data;
    using Game.Tool;
    using ECS.Common;

    internal class Util
    {
        static long uid = 1;
        public static long GetUnionId()
        {
            return uid++;
        }
    }

    public sealed class Unit : IPoolObject
    {
        public long UnitId { get; private set; }

        Dictionary<Type, IData> _dataDictionary = new Dictionary<Type, IData>();
        public IReadOnlyDictionary<Type, IData> DataDictionary 
        { 
            get
            {
                return _dataDictionary;
            }
        }

        public Unit()
        {
            UnitId = Util.GetUnionId();

            var unitData = PoolTool.GetData<UnitData>();
            unitData.stateTypeProperty = new ReactiveProperty<UnitStateType>(UnitStateType.None);
            _dataDictionary.Add(unitData.GetType(), unitData);
        }

        public Subject<IData> AddDataSubject = new Subject<IData>();
        public Subject<IData> RemoveDataSubject = new Subject<IData>();

        public void AddData(IData data)
        {
            var type = data.GetType();
#if DEBUG
            var tmpData = GetData(type);
            if (tmpData != null)
            {
                return;
            }
#endif

            _dataDictionary.Add(type, data);
            AddDataSubject.OnNext(data);
        }

        public void RemoveData(IData data)
        {
            var type = data.GetType();
#if DEBUG
            var tmpData = GetData(type);
            if (tmpData == null)
            {
                return;
            }
#endif

            _dataDictionary.Remove(data.GetType());
            RemoveDataSubject.OnNext(data);
        }

        public void RemoveData(Type type)
        {
#if DEBUG
            var tmpData = GetData(type);
            if (tmpData == null)
            {
                Log.W("Type {0} could not been found!", type);
                return;
            }
#endif

            var data = GetData(type);
            var poolObject = data as IPoolObject;
            if (poolObject != null)
            {
                PoolTool.ReleaseData(poolObject);
            }

            RemoveData(data);
        }

        public IData GetData(Type type)
        {
            IData data;
            if (!_dataDictionary.TryGetValue(type, out data))
            {
                return null;
            }

            return data;
        }

        public T AddData<T>() where T : class, IData, new()
        {
            T data = null;

#if DEBUG
            data = GetData<T>();
            if (data != null)
            {
                Log.W("Type {0} has been added!", typeof(T));
                return data;
            }
#endif

            data = PoolTool.GetData(typeof(T)) as T;
            AddData(data);
            return data;
        }

        public void RemoveData<T>() where T : class, IData
        {
#if DEBUG
            if (GetData<T>() == null)
            {
                Log.W("Type {0} could not been found!", typeof(T));
                return;
            }
#endif

            RemoveData(typeof(T));
        }

        public T GetData<T>() where T : IData
        {
            return (T)GetData(typeof(T));
        }

        public bool IsInUse { get; set; }
        public void Clear()
        {
            var unitData = GetData<UnitData>();
            unitData.Clear();

            _dataDictionary.Clear();
            _dataDictionary.Add(unitData.GetType(), unitData);
        }
    }
}