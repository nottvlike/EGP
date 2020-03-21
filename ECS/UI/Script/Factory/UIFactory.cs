namespace ECS.UI.Factory
{
    using UniRx;
    using ECS.Data;
    using UnityEngine;
    using ECS.UI;
    using ECS.UI.Data;
    using ECS.Factory;
    using Asset;
    using Asset.Factory;
    using Asset.Data;

    public static class UIFactory
    {
        public static void CreateUI(this UnitFactory factory, string assetPath,
             GameObject asset, UIData uiData = null)
        {
            var moduleMgr = WorldManager.Instance.Module;
            var requiredModuleGroup = moduleMgr.TagToModuleGroupType(UIConstant.UI_MODULE_GROUP_NAME) 
                | moduleMgr.TagToModuleGroupType(Constant.UNIT_MODULE_GROUP_NAME);
            var unit = factory.CreateAsset(requiredModuleGroup, asset);

            var unitMgr = WorldManager.Instance.Unit;
            if (uiData == null)
            {
                var uiCore = unitMgr.GetUnit(UIConstant.UI_CORE_UNIT_NAME);
                uiData = uiCore.GetData<UIData>();
            }

            var assetData = unit.GetData<AssetData>();
            assetData.transform.SetParent(uiData.uiRoot.transform, false);

            var unitData = unit.GetData<UnitData>();
            unitData.unitType = unitMgr.TagToUnitType(UIConstant.UI_UNIT_TYPE_NAME);
            unitData.stateTypeProperty.Subscribe(_ => 
            {
                if (_ == UnitStateType.Destroy)
                {
                    var paramData1 = unit.GetData<PanelParamData>();   
                    uiData.unitDict.Remove(paramData1.assetPath);
                    uiData.showedList.Remove(paramData1.assetPath);
                }
            }).AddTo(unitData.disposable);

            var panelData = assetData.GetComponent<PanelData>();
            unit.AddData(panelData, typeof(PanelData));

            var paramData = unit.AddData<PanelParamData>();
            paramData.assetPath = assetPath;
            if (paramData.stateTypeProperty == null)
            {
                paramData.stateTypeProperty = new ReactiveProperty<PanelStateType>(PanelStateType.None);
            }

            uiData.unitDict.Add(assetPath, unit);

            unitData.stateTypeProperty.Value = UnitStateType.Init;
        }
    }
}