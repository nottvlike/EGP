namespace ECS.Object.Factory
{
    using UniRx;
    using UnityEngine;
    using ECS.Data;
    using ECS.Object.Data;
    using ECS.Factory;
    using Asset.Factory;

    public static class ObjectFactory
    {
        static void CreateObject(this UnitFactory factory, GameObject gameObject)
        {
            var moduleMgr = WorldManager.Instance.Module;
            var requiredModuleGroup = moduleMgr.TagToModuleGroupType(ObjectConstant.OBJECT_MODULE_GROUP_NAME) 
                | moduleMgr.TagToModuleGroupType(Constant.UNIT_MODULE_GROUP_NAME);
            var unit = factory.CreateAsset(requiredModuleGroup, gameObject);

            var objectData = unit.AddData<ObjectData>();

            var unitData = unit.GetData<UnitData>();
            unitData.stateTypeProperty.Subscribe(_ => 
            {
                if (_ == UnitStateType.Destroy)
                {
                }
            }).AddTo(unitData.disposable);

            var unitMgr = WorldManager.Instance.Unit;
            unitData.unitType = unitMgr.TagToUnitType(ObjectConstant.OBJECT_UNIT_TYPE_NAME);
            unitData.stateTypeProperty.Value = UnitStateType.Init;
        }
    }
}