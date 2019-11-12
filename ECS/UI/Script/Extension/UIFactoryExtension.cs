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
            var unit = factory.CreateUnit();

            var obj = asset.Spawn();
            obj.transform.SetParent(uiData.uiRoot.transform);
            obj.transform.localPosition = asset.transform.localPosition;
            obj.transform.localScale = asset.transform.localScale;

            var moduleMgr = WorldManager.Instance.Module;
            var unitData = unit.GetData<UnitData>();
            unitData.unitType = WorldManager.Instance.Unit.TagToUnitType(Constant.UI_UNIT_TYPE_NAME);
            unitData.requiredModuleGroup = moduleMgr.TagToModuleGroupType(Constant.UI_MODULE_GROUP_NAME) 
                | moduleMgr.TagToModuleGroupType(Constant.UNIT_MODULE_GROUP_NAME);

            var panel = obj.GetComponent<Panel>();
            unit.AddData(panel, typeof(Panel));
            var panelData = unit.AddData<PanelData>();
            if (panelData.stateTypeProperty == null)
            {
                panelData.stateTypeProperty = new ReactiveProperty<PanelStateType>(PanelStateType.None);
            }

            uiData.unitDict.Add(assetPath, unit);
            uiData.panelDict.Add(assetPath, panel);

            unitData.stateTypeProperty.Value = UnitStateType.Init;
        }
    }
}