namespace Game.Tool
{
    using System;
    using ECS.Data;
    using ECS.Entity;
    using System.Collections.Generic;
    using ECS.Common;
    using UnityEngine;
    using ECS;
    
    public interface IPoolObject
    {
        bool IsInUse { get; set; }
        void Clear();
    }

    public sealed class PoolTool
    {
        static Dictionary<Type, List<IPoolObject>> _poolObjectDict = new Dictionary<Type, List<IPoolObject>>();

        public static T GetData<T>() where T : IData
        {
            return (T)GetData(typeof(T));
        }

        public static IData GetData(Type type)
        {
            return Get(typeof(IData), type) as IData;
        }

        public static void ReleaseData(IPoolObject data)
        {
            Release(typeof(IData), data);
        }

        public static Unit GetUnit()
        {
            var unit = Get(typeof(Unit), typeof(Unit)) as Unit;
            WorldManager.Instance.Unit.AddUnit(unit.UnitId, unit);

            return unit;
        }

        public static void ReleaseUnit(Unit unit)
        {
            var dataList = unit.DataDictionary.Values;
            foreach (var data in dataList)
            {
                var poolObject = data as IPoolObject;
                if (poolObject != null)
                {
                    ReleaseData(poolObject);
                }
            }

            WorldManager.Instance.Unit.RemoveUnit(unit.UnitId);

            Release(typeof(Unit), unit);
        }

        public static T Get<T>() where T : IPoolObject
        {
            return (T)Get(typeof(T), typeof(T));
        }

        public static void Release(IPoolObject poolObject)
        {
            Release(poolObject.GetType(), poolObject);
        }

        static IPoolObject Get(Type poolType, Type objType)
        {
            IPoolObject poolObject = null;
            List<IPoolObject> poolObjectList = null;

            if (!_poolObjectDict.TryGetValue(poolType, out poolObjectList))
            {
                poolObjectList = new List<IPoolObject>();
                _poolObjectDict.Add(poolType, poolObjectList);
            }
            else
            {
                for (var i = 0; i < poolObjectList.Count; i++)
                {
                    var tmp = poolObjectList[i];
                    if (!tmp.IsInUse && tmp.GetType() == objType)
                    {
                        tmp.IsInUse = true;
                        poolObject = tmp;
                        break;
                    }
                }
            }

            if (poolObject == null)
            {
                poolObject = Activator.CreateInstance(objType) as IPoolObject;
                poolObject.IsInUse = true;

                poolObjectList.Add(poolObject);
            }

            return poolObject;
        }

        static void Release(Type poolType, IPoolObject poolObject)
        {
            List<IPoolObject> poolObjectList = null;
            if (_poolObjectDict.TryGetValue(poolType, out poolObjectList))
            {
                if (poolObjectList.IndexOf(poolObject) != -1)
                {
                    poolObject.Clear();
                    poolObject.IsInUse = false;
                }
                else
                {
                    Log.W("PoolManager Can't find PoolObject {0}!", poolObject.GetType().ToString());
                }
            }
            else
            {
                Log.W("PoolManager Can't find PoolName {0}!", poolType.ToString());
            }
        }

        public void Clear()
        {
            foreach (var poolObjectListObject in _poolObjectDict)
            {
                var poolObjectList = poolObjectListObject.Value;
                for (var i = 0; i < poolObjectList.Count;)
                {
                    if (!poolObjectList[i].IsInUse)
                    {
                        poolObjectList.RemoveAt(i);
                    }
                    else
                    {
                        i++;
                    }
                }
            }
        }

        public void Destroy()
        {
        }
    }
}