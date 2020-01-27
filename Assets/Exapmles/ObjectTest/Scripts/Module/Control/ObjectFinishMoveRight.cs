namespace Game.ObjectTest.Module.Control
{
    using ECS.Object.Module;
    using GUnit = ECS.Unit.Unit;
    using Game.ObjectTest.Data;
    using UnityEngine;

    public sealed class ObjectFinishMoveRight : ObjectKeyboardControl<ObjectFinishMoveRightData>
    {
        protected override Vector3 GetStateParam(GUnit unit)
        {
            return Vector3.zero;
        }
    }
}