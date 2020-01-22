namespace ECS.Object.Data
{
    using ECS.Data;
    using ECS.Common;

    public enum BuffStateType
    {
        None,
        Start,
        Stop
    }

    public abstract class ObjectBuffData : IData, IPoolObject
    {
        public int value;
        public float duration;

        public BuffStateType stateType;

        public bool IsInUse { get; set; }
        public virtual void Clear()
        {
            value = 0;
            duration = 0f;
            
            stateType = BuffStateType.None;
        }
    }
}