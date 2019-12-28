namespace ECS.Factory
{
    using UniRx;
    using Data;
    using UnityEngine;
    using ECS.UI;
    using Asset;
    using ECS.Common;

    public static class UIFactoryExtension
    {
        public static void CreateUI(this UnitFactory factory, string assetPath,
             GameObject asset, UIData uiData)
        {
            var moduleMgr = WorldManager.Instance.Module;
            var requiredModuleGroup = moduleMgr.TagToModuleGroupType(Constant.UI_MODULE_GROUP_NAME) 
                | moduleMgr.TagToModuleGroupType(Constant.UNIT_MODULE_GROUP_NAME);
            var unit = factory.CreateUnit(requiredModuleGroup);

            var obj = asset.Spawn();
            obj.transform.SetParent(uiData.uiRoot.transform, false);

            var unitData = unit.GetData<UnitData>();
            unitData.unitType = WorldManager.Instance.Unit.TagToUnitType(Constant.UI_UNIT_TYPE_NAME);

            var panel = obj.GetComponent<Panel>();
            unit.AddData(panel, typeof(Panel));
            var panelData = unit.AddData<PanelData>();
            if (panelData.stateTypeProperty == null)
            {
                panelData.stateTypeProperty = new ReactiveProperty<PanelStateType>(PanelStateType.None);
            }

            unit.AddData<TaskData>();

            uiData.unitDict.Add(assetPath, unit);

            unitData.stateTypeProperty.Value = UnitStateType.Init;
        }
    }
}