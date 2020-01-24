namespace ECS.Object.Module
{
    using GUnit = ECS.Unit.Unit;
    using ECS.Module;
    using ECS.Object.Data;

    public sealed class ObjectAttribute : Module
    {
        public static void AddBase(GUnit unit, int type, float value)
        {
            var objectAttributeData = unit.GetData<ObjectAttributeData>();
            ref var attributeInfo = ref objectAttributeData.attributeInfoList[type];
            attributeInfo.baseValue += value;

            UpdateAttributeInfo(ref attributeInfo);
        }

        public static void AddBasePercent(GUnit unit, int type, float value)
        {
            var objectAttributeData = unit.GetData<ObjectAttributeData>();
            ref var attributeInfo = ref objectAttributeData.attributeInfoList[type];
            attributeInfo.basePercent += value;

            UpdateAttributeInfo(ref attributeInfo);
        }

        static void UpdateAttributeInfo(ref ObjectAttributeInfo attributeInfo)
        {
            attributeInfo.allValue = attributeInfo.baseValue * attributeInfo.basePercent;
        }
    }
}