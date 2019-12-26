namespace ECS.Entity
{
    using System;
    using System.Collections.Generic;
    using UniRx;
    using ECS.Data;
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
        public IReadOnlyDictionary<Type, IData> DataDictionary => _dataDictionary;

        UnitData _unitData;

        public Unit()
        {
            UnitId = Util.GetUnionId();

            _unitData = DataPool.Get(typeof(UnitData)) as UnitData;
            _unitData.stateTypeProperty = new ReactiveProperty<UnitStateType>(UnitStateType.None);

            _dataDictionary.Add(_unitData.GetType(), _unitData);

            _addDataSubject = new Subject<IData>();
            _removeDataSubject = new Subject<IData>();
        }

        ~Unit()
        {
            _addDataSubject.OnCompleted();
            _removeDataSubject.OnCompleted();
        }

        public IObservable<IData> ObserverAddData=> _addDataSubject;
        public IObservable<IData> ObserverRemoveData => _removeDataSubject;

        Subject<IData> _addDataSubject = new Subject<IData>();
        Subject<IData> _removeDataSubject = new Subject<IData>();

        public void AddData(IData data, Type key = null)
        {
            var type = key == null ? data.GetType() : key;
#if DEBUG
            var tmpData = GetData(type);
            if (tmpData != null)
            {
                return;
            }
#endif

            _dataDictionary.Add(type, data);
            _addDataSubject.OnNext(data);
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

            _dataDictionary.Remove(type);
            _removeDataSubject.OnNext(data);
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
                DataPool.Release(poolObject);
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

        public T AddData<T>() where T : IData
        {
            T data;

#if DEBUG
            data = GetData<T>();
            if (data != null)
            {
                Log.W("Type {0} has been added!", typeof(T));
                return data;
            }
#endif
            
            data = (T)DataPool.Get(typeof(T));
            AddData(data);
            return data;
        }

        public void RemoveData<T>() where T : IData
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
            _dataDictionary.Clear();
            _dataDictionary.Add(_unitData.GetType(), _unitData);
        }
    }
}