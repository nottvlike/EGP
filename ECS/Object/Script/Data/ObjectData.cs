namespace ECS.Data
{
    using ECS.Common;

    public enum ObjectType
    {
        None,
        Decoration,
        Trap,
        Actor,
    }

    public enum ObjectRelationType
    {
        None,
        Self,
        Friend,
        Enemy
    }

    public class ObjectData : IData, IPoolObject
    {
        public ObjectType type;
        public int camp;

        public bool IsInUse { get; set; }
        public void Clear()
        {
            type = ObjectType.None;
            camp = 0;
        }
    }
}