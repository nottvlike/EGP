namespace ECS.UI.Module
{
    using UniRx;
    using GUnit = ECS.Unit.Unit;
    using ECS.Module;
    using ECS.Data;
    using ECS.UI.Data;
    using System;
    using System.Collections.Generic;

    public abstract class UIModule : Module
    {
        public override int Group { get; protected set; } 
            = WorldManager.Instance.Module.TagToModuleGroupType(UIConstant.UI_MODULE_GROUP_NAME);

        protected override void OnAdd(GUnit unit)
        {
            var unitData = unit.GetData<UnitData>();
            var paramData = unit.GetData<PanelParamData>();
            var panelData = unit.GetData<PanelData>();
            paramData.stateTypeProperty.Subscribe(state => 
            {
                if (state == PanelStateType.Preload)
                {
                    var taskData = DataPool.Get<TaskData>();
                    OnPreload(unit, taskData.taskList);
                    TaskModule.Start(taskData, () =>
                    {
                        DataPool.Release(taskData);

                        UIManagerInstance.Instance.ShowImmediate(paramData.assetPath);
                    });
                }
                else if (state == PanelStateType.Show)
                {
                    Show(unit, panelData, paramData);
                }
                else if (state == PanelStateType.Hide)
                {
                    Hide(unit, panelData);
                }
            }).AddTo(unitData.disposable);
            
            Init(unit, panelData);
        }

        protected virtual void OnPreload(GUnit unit, List<IObservable<Unit>> taskList) { }

        protected virtual void OnInit(GUnit unit) { }
        protected virtual void OnShow(GUnit unit, PanelData panel, params object[] args) { }
        protected virtual void OnHide(GUnit unit, PanelData panel) { }

        void Init(GUnit unit, PanelData panel)
        {
            panel.transform.SetSiblingIndex(panel.Order);
            panel.gameObject.SetActive(false);

            OnInit(unit);
        }

        void Show(GUnit unit, PanelData panelData, PanelParamData paramData)
        {
            panelData.gameObject.SetActive(true);
            OnShow(unit, panelData, paramData.paramsList.ToArray());
            if (panelData.AnimationType == UIAnimationType.Animation)
            {
                panelData.AnimationProcessor?.Play(UIConstant.OPEN_TWEEN_NAME);
            }
            else if (panelData.AnimationType == UIAnimationType.Tween)
            {
                panelData.TweenProcessor?.Play(UIConstant.OPEN_TWEEN_NAME);
            }
        }

        void Hide(GUnit unit, PanelData panel)
        {
            Action hideAction = () => 
            {
                OnHide(unit, panel);
                
                if (panel.DestroyWhenHide)
                {
                    var unitData = unit.GetData<UnitData>();
                    unitData.stateTypeProperty.Value = UnitStateType.Destroy;
                }
                else
                {
                    panel.gameObject.SetActive(false);
                }
            };

            var uiCore = WorldManager.Instance.Unit.GetUnit(UIConstant.UI_CORE_UNIT_NAME);
            var taskData = uiCore.GetData<TaskData>();

            if (panel.AnimationType == UIAnimationType.Animation && panel.AnimationProcessor != null)
            {
                taskData.taskList.Add(panel.AnimationProcessor.PlayAsObserable(UIConstant.CLOSE_TWEEN_NAME).Do(_ =>
                {
                    hideAction.Invoke();
                }));
            }
            else if (panel.AnimationType == UIAnimationType.Tween && panel.TweenProcessor != null)
            {
                taskData.taskList.Add(panel.TweenProcessor.PlayAsObserable(UIConstant.CLOSE_TWEEN_NAME).Do(_ => 
                {
                    hideAction.Invoke();
                }));
            }
            else
            {
                hideAction.Invoke();
            }
        }
    }
}