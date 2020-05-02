namespace Game.ObjectTest.Module
{
    using ECS.Module;
    using GUnit = ECS.Unit.Unit;
    using Game.ObjectTest.Data;
    using System;
    using UniRx;

    public sealed class ObjectSlowDownBuff : ObjectBuff<ObjectSlowDownBuffData>
    {
        protected override void OnStart(GUnit unit, ObjectSlowDownBuffData buffData)
        {
            var moveSpeedData = unit.GetData<ObjectMoveSpeedData>();
            moveSpeedData.basePercent -= buffData.value;

            if (buffData.duration > 0)
            {
                IDisposable delayDispose = null;
                delayDispose = Observable.Timer(TimeSpan.FromSeconds(buffData.duration)).Subscribe(_ =>
                {
                    delayDispose?.Dispose();

                    Stop(unit, buffData);
                });
            }
        }

        protected override void OnStop(GUnit unit, ObjectSlowDownBuffData buffData)
        {
            var moveSpeedData = unit.GetData<ObjectMoveSpeedData>();
            moveSpeedData.basePercent += buffData.value;
        }
    }
}