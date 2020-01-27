namespace Game.ObjectTest.Module.Control
{
    using ECS.Object.Module;
    using ECS.Object.Data;
    using GUnit = ECS.Unit.Unit;
    using Game.ObjectTest.Data;
    using UnityEngine;
    using System;

    public sealed class ObjectMoveLeft : ObjectKeyboardControl<ObjectMoveLeftData>
    {
        protected override Vector3 GetStateParam(GUnit unit)
        {
            return Vector3.left;
        }
    }
}