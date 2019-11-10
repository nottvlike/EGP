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
            if (!typeof(IData).IsAssignableFrom(type))
            {
                Log.E("Type should extrend IData!");
                return null;
            }
#endif
            if (!typeof(IPoolObject).IsAssignableFrom(type))
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