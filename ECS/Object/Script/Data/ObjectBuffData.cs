namespace ECS.Object.Data
{
    using ECS.Data;
    using ECS.Common;
    using ECS.Object.Module;
    using System.Collections.Generic;
    using UniRx;

    public enum BuffStateType
    {
        None,
        Start,
        Stop
    }

    public abstract class ObjectBuffData : IPoolObject
    {
        public float value;
        public float duration;
        public int maxOverlay = 1;
        
        public bool IsInUse { get; set; }
        public virtual void Clear()
        {
            value = 0;
            duration = 0f;
            maxOverlay = 1;
        }
    }


    public class ObjectBuffProcessData : IData, IPoolObject
    {
        public ReactiveCollection<ObjectBuffData> currentBuffList = new ReactiveCollection<ObjectBuffData>();
        public List<IObjectBuff> allBuffModuleList = new List<IObjectBuff>();

        public bool IsInUse { get; set; }
        public virtual void Clear()
        {
            currentBuffList.Clear();
            allBuffModuleList.Clear();
        }
    }
}