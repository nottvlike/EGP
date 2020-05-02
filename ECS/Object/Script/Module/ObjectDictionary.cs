namespace ECS.Module
{
    using System.Collections.Generic;

    public abstract class ObjectDictionary<T1,T2> : SingleModule
    {
        static Dictionary<T1, T2> _dictionary = new Dictionary<T1, T2>();

        public static void Set(T1 value1, T2 value2)
        {
            _dictionary[value1] = value2;
        }

        public static T2 Get(T1 value1)
        {
            return _dictionary[value1];
        }

        public static void Clear()
        {
            _dictionary.Clear();
        }

        public static int Count()
        {
            return _dictionary.Count;
        }
    }
}