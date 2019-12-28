namespace ECS.UI
{
    using ECS.Common;
    using ECS.Data;
    using ECS.Factory;
    using UniRx;
    using System;
    using Asset;
    using UnityEngine;
    
    internal class UIManagerInstance : Singleton<UIManagerInstance>
    {
        UIData _uiData { get; set; }

        WorldManager worldMgr => WorldManager.Instance;

        void InitUICore()
        {
            var requiredModuleGroup = worldMgr.Module.TagToModuleGroupType(Constant.SYSTEM_MODULE_GROUP_NAME) 
                | worldMgr.Module.TagToModuleGroupType(Constant.UI_MODULE_GROUP_NAME);
            var unit = worldMgr.Factory.CreateUnit(requiredModuleGroup);

            _uiData = new UIData();
            unit.AddData(_uiData);

            var unitData = unit.GetData<UnitData>();
            unitData.unitType = worldMgr.Unit.TagToUnitType(Constant.UI_UNIT_TYPE_NAME);
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
            .ContinueWith(_ =>
            {
                var unit = _uiData.unitDict[assetPath];
                var taskData = unit.GetData<TaskData>();
                if (taskData != null && taskData.taskList.Count > 0)
                {
                    return taskData.taskList.Merge(taskData.maxConcurrent).AsUnitObservable().Finally(() =>
                    {
                        taskData.taskList.Clear();
                    });
                }
                else
                {
                    return Observable.ReturnUnit();
                }
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
            var panelData = unit.GetData<PanelData>();
            panelData.paramsList.Clear();
            if (args != null)
            {
                panelData.paramsList.AddRange(args);
            }

            var panel = unit.GetData<Panel>();
#if DEBUG
            if (!forceUpdateWhenShowed && IsShowed(assetPath))
            {
                Log.W("PanelData {0} has been showed!", assetPath);
                return;
            }
#endif

            if (panel.PanelMode != PanelMode.Alone)
            {
                ShowImpl(assetPath, panelData);
            }
            else
            {
                for (var i = 0; i < _uiData.showedList.Count;)
                {
                    var showed = _uiData.showedList[i];
                    HideImpl(showed);
                }

                ShowImpl(assetPath, panelData);
            }

            return;
        }

       void ShowImpl(string assetPath, PanelData panelData)
        {
            if (_uiData.showedList.IndexOf(assetPath) !=-1)
            {
                panelData.stateTypeProperty.SetValueAndForceNotify(PanelStateType.Show);
            }
            else
            {
                _uiData.showedList.Add(assetPath);
                panelData.stateTypeProperty.Value = PanelStateType.Show;
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
            return;
        }

        void HideImpl(string assetPath)
        {
            _uiData.showedList.Remove(assetPath);

            var unit = _uiData.unitDict[assetPath];
            var panelData = unit.GetData<PanelData>();
            panelData.stateTypeProperty.Value = PanelStateType.Hide;
        }
    }
}