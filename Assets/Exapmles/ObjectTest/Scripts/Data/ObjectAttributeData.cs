namespace Game.ObjectTest.Data
{
    using ECS.Object.Data;
    using Game;
    
    public class ObjectMoveSpeedData : ObjectAttributeData
    {
        public override string name { get { return ObjectTestConstant.ATTRIBUTE_MOVE_SPEED_NAME; } }
        public override string description { get { return ObjectTestConstant.ATTRIBUTE_MOVE_SPEED_DESCRIPTION; } }
    }
}