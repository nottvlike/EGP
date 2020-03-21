namespace ECS.UI
{
    using ECS.Common;
    using ECS.Data;
    using ECS.Module;
    using ECS.UI.Factory;
    using ECS.UI.Data;
    using UniRx;
    using System;
    using Asset;
    using UnityEngine;
    
    internal class UIManagerInstance : Singleton<UIManagerInstance>
    {
        UIData _uiData { get; set; }
        TaskData _taskData { get; set; }

        WorldManager worldMgr => WorldManager.Instance;

        void InitUICore()
        {
            var requiredModuleGroup = worldMgr.Module.TagToModuleGroupType(Constant.SYSTEM_MODULE_GROUP_NAME) 
                | worldMgr.Module.TagToModuleGroupType(UIConstant.UI_MODULE_GROUP_NAME);
            var unit = worldMgr.Factory.CreateUnit(requiredModuleGroup);

            _uiData = unit.AddData<UIData>();
            _taskData = unit.AddData<TaskData>();

            var unitData = unit.GetData<UnitData>();
            unitData.unitType = worldMgr.Unit.TagToUnitType(UIConstant.UI_UNIT_TYPE_NAME);
            unitData.tag = UIConstant.UI_CORE_UNIT_NAME;

            unitData.stateTypeProperty.Value = UnitStateType.Init;
        }

        public void Init()
        {
            InitUICore();
        }

        public IObservable<Unit> LoadUIRoot()
        {
            if (_uiData.uiRoot != null)
            {
                return Observable.ReturnUnit();
            }

            return AssetManager.Load<GameObject>(UIConstant.UI_ROOT_ASSET_NAME).Do(root => 
            {
                _uiData.uiRoot = root.Spawn();
                GameObject.DontDestroyOnLoad(_uiData.uiRoot);
            }).AsUnitObservable();
        }

        public IObservable<Unit> LoadUI(string assetPath)
        {
            if (_uiData.unitDict.ContainsKey(assetPath))
            {
                return Observable.ReturnUnit();
            }

            return AssetManager.Load<GameObject>(assetPath)
            .Do(asset => 
            {
                worldMgr.Factory.CreateUI(assetPath, asset, _uiData);
            })
            .AsUnitObservable();
        }

        public bool IsShowed(string assetPath)
        {
            return _uiData.showedList.IndexOf(assetPath) != -1;
        }

        public void Show(string assetPath, bool forceUpdateWhenShowed, params object[] args)
        {
            var unit = _uiData.unitDict[assetPath];
            var paramData = unit.GetData<PanelParamData>();
            paramData.paramsList.Clear();
            if (args != null)
            {
                paramData.paramsList.AddRange(args);
            }

#if DEBUG
            if (!forceUpdateWhenShowed && IsShowed(assetPath))
            {
                Log.W("PanelData {0} has been showed!", assetPath);
                return;
            }
#endif
            paramData.stateTypeProperty.Value = PanelStateType.Preload;
        }

        public void ShowImmediate(string assetPath)
        {
            var unit = _uiData.unitDict[assetPath];
            var panelData = unit.GetData<PanelData>();
            var paramData = unit.GetData<PanelParamData>();

            if (panelData.PanelMode != PanelMode.Alone)
            {
                ShowImpl(assetPath, paramData);
            }
            else
            {
                for (var i = 0; i < _uiData.showedList.Count;)
                {
                    var showed = _uiData.showedList[i];
                    HideImpl(showed);
                }

                TaskModule.Start(_taskData, () =>
                {
                    ShowImpl(assetPath, paramData);
                });
            }
        }

        void ShowImpl(string assetPath, PanelParamData paramData)
        {
            if (_uiData.showedList.IndexOf(assetPath) !=-1)
            {
                paramData.stateTypeProperty.SetValueAndForceNotify(PanelStateType.Show);
            }
            else
            {
                _uiData.showedList.Add(assetPath);
                paramData.stateTypeProperty.Value = PanelStateType.Show;
            }
        }

        public void Hide(string assetPath)
        {
#if DEBUG
            if (_uiData.uiRoot == null)
            {
                Log.E("Hide panel {0} failed, ui root is null!", assetPath);
                return;
            }

            if (!IsShowed(assetPath))
            {
                Log.W("Hide panel {0} failed, panel is not showed!", assetPath);
                return;
            }
#endif
            HideImpl(assetPath);

            TaskModule.Start(_taskData);
        }

        void HideImpl(string assetPath)
        {
            _uiData.showedList.Remove(assetPath);

            var unit = _uiData.unitDict[assetPath];
            var paramData = unit.GetData<PanelParamData>();
            paramData.stateTypeProperty.Value = PanelStateType.Hide;
        }
    }
}