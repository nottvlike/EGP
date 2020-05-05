namespace Game.ObjectTest.Data
{
    using ECS.Data;
    
    public class ObjectSlowDownBuffData : IBuffData
    {
        public float value;
        public float duration;

        public bool IsInUse { get; set; }
        public void Clear()
        {
            value = 0f;
            duration = 0f;
        }
    }
}