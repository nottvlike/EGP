namespace ECS.Data
{
    using ECS.Common;
    using System;

    public sealed class DataPool
    {
        public static T Get<T>() where T : class, IData, IPoolObject
        {
            return Pool.Get<T>();
        }

        public static IData Get(Type type)
        {
#if DEBUG
            if (type != typeof(IData))
            {
                Log.E("Type should extrend IData!");
                return null;
            }
#endif
            if (type != typeof(IPoolObject))
            {
                return (IData)Activator.CreateInstance(type);
            }
            else
            {
                return (IData)Pool.Get(type);
            }
        }

        public static void Release(IPoolObject data)
        {
            Pool.Release(data);
        }
    }
}