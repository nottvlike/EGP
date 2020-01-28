namespace Game.ObjectTest.Data
{
    using ECS.Data;
    using ECS.Common;

    public class SlowDownTrapData : IData, IPoolObject
    {
        public bool IsInUse { get; set; }
        public virtual void Clear() {}
    }
}