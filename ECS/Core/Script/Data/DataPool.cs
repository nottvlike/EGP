namespace ECS.Data
{
    using ECS.Common;
    using System;

    public sealed class DataPool
    {
        public static T Get<T>() where T : IData
        {
            return (T)Pool.Get(typeof(T));
        }

        public static IData Get(Type type)
        {
            return (IData)Pool.Get(type);
        }

        public static void Release(IPoolObject data)
        {
            Pool.Release(data);
        }
    }
}