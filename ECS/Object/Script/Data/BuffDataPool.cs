namespace ECS.Object.Data
{
    using ECS.Common;
    using System;

    public sealed class BuffDataPool
    {
        public static T Get<T>() where T : ObjectBuffData
        {
            return (T)Get(typeof(T));
        }

        public static ObjectBuffData Get(Type type)
        {
#if DEBUG
            if (!typeof(ObjectBuffData).IsAssignableFrom(type))
            {
                Log.E("Type should extrend ObjectBuffData!");
                return null;
            }
#endif
            return (ObjectBuffData)Pool.Get(type);
        }

        public static void Release(IPoolObject data)
        {
            Pool.Release(data);
        }
    }
}