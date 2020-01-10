namespace ECS.Object.Module
{
    using GUnit = ECS.Unit.Unit;
    using ECS.Module;
    using ECS.Object.Data;

    public sealed class ObjectModule : Module
    {
        public ObjectRelationType GetReleation(GUnit unit1, GUnit unit2)
        {
            return ObjectRelationType.None;
        }
    }
}