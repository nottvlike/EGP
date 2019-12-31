namespace ECS.Factory
{
    using UniRx;
    using Data;
    using UnityEngine;
    using ECS.UI;
    using GUnit = ECS.Unit.Unit;
    using Asset;

    public static class UIFactoryExtension
    {
        public static void CreateUI(this UnitFactory factory, string assetPath,
             GameObject asset, UIData uiData = null)
        {
            var moduleMgr = WorldManager.Instance.Module;
            var requiredModuleGroup = moduleMgr.TagToModuleGroupType(Constant.UI_MODULE_GROUP_NAME) 
                | moduleMgr.TagToModuleGroupType(Constant.UNIT_MODULE_GROUP_NAME);
            var unit = factory.CreateUnit(requiredModuleGroup);

            var unitMgr = WorldManager.Instance.Unit;
            if (uiData == null)
            {
                var uiCore = unitMgr.GetUnit(UIConstant.UI_CORE_UNIT_NAME);
                uiData = uiCore.GetData<UIData>();
            }

            var obj = asset.Spawn();
            obj.transform.SetParent(uiData.uiRoot.transform, false);

            var unitData = unit.GetData<UnitData>();
            unitData.unitType = unitMgr.TagToUnitType(Constant.UI_UNIT_TYPE_NAME);
            unitData.stateTypeProperty.Subscribe(_ => 
            {
                if (_ == UnitStateType.Destroy)
                {
                    factory.DestroyUIUnit(unit, uiData);
                }
            });

            var panel = obj.GetComponent<Panel>();
            unit.AddData(panel, typeof(Panel));

            var panelData = unit.AddData<PanelData>();
            panelData.assetPath = assetPath;
            if (panelData.stateTypeProperty == null)
            {
                panelData.stateTypeProperty = new ReactiveProperty<PanelStateType>(PanelStateType.None);
            }

            unit.AddData<TaskData>();

            uiData.unitDict.Add(assetPath, unit);

            unitData.stateTypeProperty.Value = UnitStateType.Init;
        }

        public static void DestroyUIUnit(this UnitFactory factory, GUnit unit, UIData uiData = null)
        {
            if (uiData == null)
            {
                var uiCore = WorldManager.Instance.Unit.GetUnit(UIConstant.UI_CORE_UNIT_NAME);
                uiData = uiCore.GetData<UIData>();
            }

            var panelData = unit.GetData<PanelData>();   
            uiData.unitDict.Remove(panelData.assetPath);

            var panel = unit.GetData<Panel>();
            panel.gameObject.Despawn();

            factory.DestroyUnit(unit);
        }
    }
}