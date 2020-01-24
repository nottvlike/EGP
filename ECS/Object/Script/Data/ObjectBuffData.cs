namespace ECS.Object.Data
{
    using ECS.Data;
    using ECS.Common;
    using UniRx;

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

        public ReactiveProperty<BuffStateType> stateTypeProperty = new ReactiveProperty<BuffStateType>(BuffStateType.None);

        public bool IsInUse { get; set; }
        public virtual void Clear()
        {
            value = 0;
            duration = 0f;

            stateTypeProperty.Value = BuffStateType.None;
        }
    }
}