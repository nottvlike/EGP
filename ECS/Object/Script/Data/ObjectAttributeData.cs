namespace ECS.Object.Data
{
    using ECS.Data;
    using ECS.Common;

    public abstract class ObjectAttributeData : IData, IPoolObject
    {
        public virtual string name { get; }

        public float baseValue;
        public float basePercent;

        public float allValue { get { return baseValue * basePercent; } }

        public bool IsInUse { get; set; }
        public void Clear()
        {
            baseValue = 0f;
            basePercent = 0f;
        }
    }
}