namespace ECS.Data
{
    using ECS.Common;
    using ECS.Module;
    using System.Collections.Generic;
    using UniRx;

    public interface IBuffData : IPoolObject {}
    public interface IOverlayBuffData : IBuffData
    {
        int overlay { get; set; }
        int maxOverlay { get; set; }
    }

    public class ObjectBuffProcessData : IData, IPoolObject
    {
        public List<IBuffData> currentBuffDataList = new List<IBuffData>();

        public bool IsInUse { get; set; }
        public virtual void Clear()
        {
            currentBuffDataList.Clear();
        }
    }
}