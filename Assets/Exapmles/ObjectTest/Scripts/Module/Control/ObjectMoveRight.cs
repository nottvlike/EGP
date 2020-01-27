namespace Game.ObjectTest.Module.Control
{
    using ECS.Object.Module;
    using ECS.Object.Data;
    using GUnit = ECS.Unit.Unit;
    using Game.ObjectTest.Data;
    using UnityEngine;
    using System;

    public sealed class ObjectMoveRight : ObjectKeyboardControl<ObjectMoveRightData>
    {
        protected override Vector3 GetStateParam(GUnit unit)
        {
            return Vector3.right;
        }
    }
}