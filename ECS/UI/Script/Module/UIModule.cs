﻿namespace ECS.UI.Module
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
            var panelData = unit.GetData<PanelData>();
            var panel = unit.GetData<Panel>();
            panelData.stateTypeProperty.Subscribe(state => 
            {
                if (state == PanelStateType.Preload)
                {
                    var taskData = DataPool.Get<TaskData>();
                    OnPreload(unit, taskData.taskList);
                    TaskModule.Start(taskData, () =>
                    {
                        DataPool.Release(taskData);

                        UIManagerInstance.Instance.ShowImmediate(panelData.assetPath);
                    });
                }
                else if (state == PanelStateType.Show)
                {
                    Show(unit, panel, panelData);
                }
                else if (state == PanelStateType.Hide)
                {
                    Hide(unit, panel);
                }
            }).AddTo(unitData.disposable);
            
            Init(unit, panel);
        }

        protected virtual void OnPreload(GUnit unit, List<IObservable<Unit>> taskList) { }

        protected virtual void OnInit(GUnit unit) { }
        protected virtual void OnShow(GUnit unit, Panel panel, params object[] args) { }
        protected virtual void OnHide(GUnit unit, Panel panel) { }

        void Init(GUnit unit, Panel panel)
        {
            panel.transform.SetSiblingIndex(panel.Order);
            panel.gameObject.SetActive(false);

            OnInit(unit);
        }

        void Show(GUnit unit, Panel panel, PanelData panelData)
        {
            panel.gameObject.SetActive(true);
            OnShow(unit, panel, panelData.paramsList.ToArray());
            if (panel.AnimationType == UIAnimationType.Animation)
            {
                panel.AnimationProcessor?.Play(UIConstant.OPEN_TWEEN_NAME);
            }
            else if (panel.AnimationType == UIAnimationType.Tween)
            {
                panel.TweenProcessor?.Play(UIConstant.OPEN_TWEEN_NAME);
            }
        }

        void Hide(GUnit unit, Panel panel)
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