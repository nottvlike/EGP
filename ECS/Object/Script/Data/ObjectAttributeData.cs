namespace ECS.Object.Data
{
    using ECS.Data;
    using ECS.Common;

    public enum ObjectAttributeType
    {
        MaxHP,
        MaxMP,
        HP,
        MP,
        ATK,
        DEF,
    }

    public struct ObjectAttributeInfo
    {
        public int type;
        public float baseValue;
        public float basePercent;
        public float allValue;
    }

    public class ObjectAttributeData : IData, IPoolObject
    {
        public ObjectAttributeInfo[] attributeInfoList;

        public bool IsInUse { get; set; }
        public void Clear()
        {
            attributeInfoList = null;
        }
    }
}