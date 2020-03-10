namespace ECS.Object.Module
{
    using GUnit = ECS.Unit.Unit;
    using ECS.Module;
    using System;
    using System.Linq;
    using System.Collections.Generic;

    public abstract class DictionaryDataServer<T1,T2> : SingleModule
    {
        static Dictionary<ValueTuple<uint, T1>, T2> _dictionary = new Dictionary<(uint, T1), T2>();

        public static void Set(GUnit unit, T1 value1, T2 value2)
        {
            var keyValue = ValueTuple.Create(unit.UnitId, value1);
            _dictionary[keyValue] = value2;
        }

        public static T2 Get(GUnit unit, T1 value1)
        {
            var keyValue = ValueTuple.Create(unit.UnitId, value1);
            return _dictionary[keyValue];
        }

        public static void Clear(GUnit unit)
        {
            var removeList = _dictionary.Where(_ => _.Key.Item1 == unit.UnitId).Select(_ => _.Key).ToArray();
            foreach (var remove in removeList)
            {
                _dictionary.Remove(remove);
            }
        }
    }
}