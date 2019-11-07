﻿namespace ECS.Module
{
    using System.Collections.Generic;
    using System;
    using System.Linq;
    using UniRx;
    using GUnit = ECS.Entity.Unit;

    public abstract class Module
    {
        public Type[] RequiredDataList { get; protected set; }

        public virtual int Group { get; protected set; } = 0;

        public bool IsMeet(IEnumerable<Data.IData> dataList)
        {
            var typeList = dataList.Select(_ => _.GetType());
            var union = RequiredDataList.Intersect(typeList);
            return union.Count() == RequiredDataList.Length;
        }

        public bool Contains(long unitId)
        {
            return _unitIdList.IndexOf(unitId) != -1;
        }

        List<long> _unitIdList = new List<long>();
        public IReadOnlyList<long> UnitIdList { get { return _unitIdList; } }

        public void Add(GUnit unit)
        {
            _unitIdList.Add(unit.UnitId);
            OnAdd(unit);
        }

        public void Remove(GUnit unit)
        {
            _unitIdList.Remove(unit.UnitId);
            OnRemove(unit);
        }

        protected virtual void OnAdd(GUnit unit) { }
        protected virtual void OnRemove(GUnit unit) { }
    }
}