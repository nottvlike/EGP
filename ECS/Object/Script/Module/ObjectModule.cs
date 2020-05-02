namespace ECS.Module
{
    using GUnit = ECS.Unit.Unit;
    using ECS.Data;

    public sealed class ObjectModule : Module
    {
        public ObjectRelationType GetReleation(GUnit unit1, GUnit unit2)
        {
            return ObjectRelationType.None;
        }
    }
}