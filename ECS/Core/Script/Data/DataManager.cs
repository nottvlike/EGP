namespace ECS.Data
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using UniRx;
    using ECS.Common;

    internal class DataManager
    {
        Dictionary<ValueTuple<uint, Type>, IData> _dataDictionary = new Dictionary<ValueTuple<uint, Type>, IData>();
        Dictionary<ValueTuple<uint, Type>, IData> _removedDataDictionary = new Dictionary<ValueTuple<uint, Type>, IData>();

        public void AddData(uint unitId, IData data, Type key = null)
        {
            var type = key == null ? data.GetType() : key;
#if DEBUG
            var tmpData = TryGetData(unitId, type);
            if (tmpData != null)
            {
                Log.W("Add data {0} failed, data has been added to unit id : {1}!", type, unitId);
                return;
            }
#endif

            _dataDictionary.Add(ValueTuple.Create(unitId, type), data);
        }

        public void RemoveData(uint unitId, IData data, bool cacheData = false)
        {
            var type = data.GetType();
#if DEBUG
            var tmpData = TryGetData(unitId, type);
            if (tmpData == null)
            {
                Log.W("Remove data {0} failed, data doesn't exist in unit id : {1}!", type, unitId);
                return;
            }
#endif

            var dataKey = ValueTuple.Create(unitId, type);
            if (cacheData)
            {
                _removedDataDictionary.Add(dataKey, data);
            }
            _dataDictionary.Remove(dataKey);
        }

        public IData GetData(uint unitId, Type type, bool includeCachedData = false)
        {
            var dataKey = ValueTuple.Create(unitId, type);
            IData data = null;
            if (!_dataDictionary.TryGetValue(dataKey, out data)
                && (includeCachedData && !_removedDataDictionary.TryGetValue(dataKey, out data)))
            {
                Log.W("Get data {0} failed, data doesn't exist in unit id : {1}!", type, unitId);
            }

            return data;
        }

        public IData TryGetData(uint unitId, Type type, bool includeCachedData = false)
        {
            var dataKey = ValueTuple.Create(unitId, type);
            IData data = null;
            if (!_dataDictionary.TryGetValue(dataKey, out data)
                && (includeCachedData && !_removedDataDictionary.TryGetValue(dataKey, out data)))
            {
                return null;
            }

            return data;
        }

        public void ClearDataToCached(uint unitId)
        {
            var dataKeyList = _dataDictionary.Where(_ => _.Key.Item1 == unitId)
                .Select(_ => _.Key).ToArray();
            foreach (var dataKey in dataKeyList)
            {
                _removedDataDictionary.Add(dataKey, _dataDictionary[dataKey]);
                _dataDictionary.Remove(dataKey);
            }
        }

        public void ClearCachedData(uint unitId)
        {
            var removedDataKeyList = _removedDataDictionary.Where(_ => _.Key.Item1 == unitId)
                .Select(_ => _.Key).ToArray();
            foreach (var dataKey in removedDataKeyList)

            {
                var poolObject = _removedDataDictionary[dataKey] as IPoolObject;

                if (poolObject != null)
                {
                    Pool.Release(poolObject);
                }
                _removedDataDictionary.Remove(dataKey);
            }
        }

        public IEnumerable<IData> GetAllData(uint unitId)
        {
            return _dataDictionary.Where(_ => _.Key.Item1 == unitId).Select(_ => _.Value);
        }
    }
}