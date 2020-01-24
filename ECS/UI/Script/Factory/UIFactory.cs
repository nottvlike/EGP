namespace ECS.Factory
{
    using UniRx;
    using Data;
    using UnityEngine;
    using ECS.UI;
    using Asset;

    public static class UIFactory
    {
        public static void CreateUI(this UnitFactory factory, string assetPath,
             GameObject asset, UIData uiData = null)
        {
            var moduleMgr = WorldManager.Instance.Module;
            var requiredModuleGroup = moduleMgr.TagToModuleGroupType(UIConstant.UI_MODULE_GROUP_NAME) 
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
            unitData.unitType = unitMgr.TagToUnitType(UIConstant.UI_UNIT_TYPE_NAME);
            unitData.stateTypeProperty.Subscribe(_ => 
            {
                if (_ == UnitStateType.Destroy)
                {
                    var panelData1 = unit.GetData<PanelData>();   
                    uiData.unitDict.Remove(panelData1.assetPath);

                    var panel1 = unit.GetData<Panel>();
                    panel1.gameObject.Despawn();
                }
            }).AddTo(unitData.disposable);

            var panel = obj.GetComponent<Panel>();
            unit.AddData(panel, typeof(Panel));

            var panelData = unit.AddData<PanelData>();
            panelData.assetPath = assetPath;
            if (panelData.stateTypeProperty == null)
            {
                panelData.stateTypeProperty = new ReactiveProperty<PanelStateType>(PanelStateType.None);
            }

            uiData.unitDict.Add(assetPath, unit);

            unitData.stateTypeProperty.Value = UnitStateType.Init;
        }
    }
}