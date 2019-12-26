namespace ECS.Unit
{
    using ECS.Common;

    public sealed class UnitPool
    {
        public static Unit Get()
        {
            var unit = Pool.Get<Unit>();
            WorldManager.Instance.Unit.AddUnit(unit.UnitId, unit);

            return unit;
        }

        public static void Release(Unit unit)
        {
            var dataList = unit.DataDictionary.Values;
            foreach (var data in dataList)
            {
                var poolObject = data as IPoolObject;
                if (poolObject != null)
                {
                    Pool.Release(poolObject);
                }
            }

            WorldManager.Instance.Unit.RemoveUnit(unit.UnitId);

            Pool.Release(unit);
        }
    }
}