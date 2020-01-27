namespace Game.ObjectTest.Data
{
    using ECS.Object.Data;
    using System;

    public class ObjectIdleStateData : ObjectStateData
    {
        public override string name { get { return ObjectTestConstant.STATE_IDLE; } }
        public override int priority { get { return 0; } }
        public override bool isLoop { get { return true; } }
        public override bool isDefault { get { return true; } }
    }

    public class ObjectMoveStateData : ObjectStateData
    {
        public override string name { get { return ObjectTestConstant.STATE_MOVE; } }
        public override int priority { get { return 1; } }
        public override bool isLoop { get { return true; } }

        public IDisposable moveDispose;

        public override void Clear()
        {
            base.Clear();
            moveDispose = null;
        }
    }
}