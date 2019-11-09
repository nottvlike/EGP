namespace ECS.Common
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using System.Linq;
    
    public interface IPoolObject
    {
        bool IsInUse { get; set; }
        void Clear();
    }

    public sealed class Pool
    {
        static Dictionary<Type, List<IPoolObject>> _poolObjectDict = new Dictionary<Type, List<IPoolObject>>();

        public static IPoolObject Get(Type objType)
        {
            return GetImpl(objType);
        }

        public static T Get<T>() where T : class, IPoolObject
        {
            return Get(typeof(T)) as T;
        }

        public static void Release(IPoolObject poolObject)
        {
            ReleaseImpl(poolObject);
        }

        public void ClearUnusedPoolObject()
        {
            var list = _poolObjectDict.Keys.ToArray();
            foreach (var poolType in list)
            {
                var poolObjectList = _poolObjectDict[poolType];
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

                if (poolObjectList.Count == 0)
                {
                    _poolObjectDict.Remove(poolType);
                }
            }
        }
        
        static IPoolObject GetImpl(Type objType)
        {
            IPoolObject poolObject = null;
            List<IPoolObject> poolObjectList = null;

            if (!_poolObjectDict.TryGetValue(objType, out poolObjectList))
            {
                poolObjectList = new List<IPoolObject>();
                _poolObjectDict.Add(objType, poolObjectList);
            }
            else
            {
                for (var i = 0; i < poolObjectList.Count; i++)
                {
                    var tmp = poolObjectList[i];
                    if (!tmp.IsInUse)
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

        static void ReleaseImpl(IPoolObject poolObject)
        {
            var poolType = poolObject.GetType();
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
    }
}