namespace ECS.Module
{
    using GUnit = ECS.Unit.Unit;
    using ECS.Data;
    using ECS.Common;
    using ECS.Factory;
    using UniRx;
    using System;
    using UnityEngine;

    public class UIProcess : SingleModule
    {
        public override int Group { get; protected set; } 
            = WorldManager.Instance.Module.TagToModuleGroupType(UIConstant.UI_MODULE_GROUP_NAME);

        public UIProcess()
        {
            RequiredDataList = new Type[]{
                typeof(UIProcessData)
            };
        }

        static UIProcessData _uiData;
        static TaskData _taskData;

        protected override void OnAdd(GUnit unit)
        {
            _uiData = unit.GetData<UIProcessData>();
            _taskData = unit.GetData<TaskData>();
        }

        protected override void OnRemove(GUnit unit)
        {
            _uiData = null;
            _taskData = null;
        }

        static IObservable<Unit> LoadUIRoot()
        {
            if (_uiData.uiRoot != null)
            {
                return Observable.ReturnUnit();
            }

            return AssetProcess.Load<GameObject>(UIConstant.UI_ROOT_ASSET_NAME).Do(root => 
            {
                _uiData.uiRoot = root.Spawn();
                GameObject.DontDestroyOnLoad(_uiData.uiRoot);
            }).AsUnitObservable();
        }

        static IObservable<Unit> LoadUI(string assetPath)
        {
            if (_uiData.unitDict.ContainsKey(assetPath))
            {
                return Observable.ReturnUnit();
            }

            return AssetProcess.Load<GameObject>(assetPath)
            .Do(asset => 
            {
                WorldManager.Instance.Factory.CreateUI(assetPath, asset, _uiData);
            })
            .AsUnitObservable();
        }

        public static bool IsShowed(string assetPath)
        {
            return _uiData.showedList.IndexOf(assetPath) != -1;
        }

        public static IObservable<Unit> ShowAsObservable(string assetPath, bool forceUpdateWhenShowed = false, params object[] args)
        {
            return LoadUIRoot().ContinueWith(_ => LoadUI(assetPath))
                .Do(_ => Preload(assetPath, forceUpdateWhenShowed, args));
        }

        public static void Show(string assetPath, bool forceUpdateWhenShowed = false, params object[] args)
        {
            ShowAsObservable(assetPath, forceUpdateWhenShowed, args).Subscribe();
        }

        static void Preload(string assetPath, bool forceUpdateWhenShowed, params object[] args)
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

        internal static void ShowImmediate(string assetPath)
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

        static void ShowImpl(string assetPath, PanelParamData paramData)
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

        public static void Hide(string assetPath)
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

        static void HideImpl(string assetPath)
        {
            _uiData.showedList.Remove(assetPath);

            var unit = _uiData.unitDict[assetPath];
            var paramData = unit.GetData<PanelParamData>();
            paramData.stateTypeProperty.Value = PanelStateType.Hide;
        }
    }
}