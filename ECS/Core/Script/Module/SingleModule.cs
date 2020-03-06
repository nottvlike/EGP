namespace ECS.Module
{
    using System.Collections.Generic;
    using System;
    using System.Linq;
    using UniRx;
    using GUnit = ECS.Unit.Unit;

    public abstract class SingleModule : IModule
    {
        public Type[] RequiredDataList { get; protected set; }

        public virtual int Group { get; protected set; } = 0;

        public bool IsMeet(IEnumerable<Data.IData> dataList)
        {
            var typeList = dataList.Select(_ => _.GetType());
            var union = RequiredDataList.Intersect(typeList);
            return union.Count() == RequiredDataList.Length;
        }

        public bool Contains(uint unitId)
        {
            return _unitId == unitId;
        }

        uint _unitId;

        public void Add(GUnit unit)
        {
            _unitId = unit.UnitId;
            OnAdd(unit);
        }

        public void Remove(GUnit unit)
        {
            _unitId = 0;
            OnRemove(unit);
        }

        protected virtual void OnAdd(GUnit unit) { }
        protected virtual void OnRemove(GUnit unit) { }
    }
}