namespace ECS.Module
{
    using System.Collections.Generic;
    using System;
    using ECS.Data;
    using GUnit = ECS.Unit.Unit;

    public interface IModule
    {
        Type[] RequiredDataList { get; }
        int Group { get; }

        bool IsMeet(IEnumerable<IData> dataList);

        bool Contains(uint unitId);

        void Add(GUnit unit);

        void Remove(GUnit unit);
    }
}