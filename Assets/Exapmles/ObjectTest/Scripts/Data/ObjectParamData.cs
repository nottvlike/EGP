namespace Game.ObjectTest.Data
{
    using ECS.Data;
    using ECS.Common;
    using System;

    public class ObjectMoveParamData : IData, IPoolObject
    {
        public IDisposable moveDispose;

        public bool IsInUse { get; set; }
        public void Clear()
        {
            moveDispose = null;
        }
    }
}