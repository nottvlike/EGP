namespace ECS.Object.Data
{
    using ECS.Data;
    using ECS.Common;
    using System.Collections.Generic;

    public struct AttributeInfo
    {
        public int type;
        public float baseValue;
        public float modifyValue;
        public float finalValue;
    }

    public class ObjectAttributeData : IData, IPoolObject
    {
        public List<AttributeInfo> attributeInfoList;

        public bool IsInUse { get; set; }
        public void Clear()
        {
            attributeInfoList.Clear();
        }
    }
}