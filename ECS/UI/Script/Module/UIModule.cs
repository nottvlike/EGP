namespace ECS.UI
{
    using UniRx;
    using GUnit = ECS.Entity.Unit;
    using ECS.Module;
    using ECS.Data;

    public abstract class UIModule : Module
    {
        public override int Group { get; protected set; } 
            = WorldManager.Instance.Module.TagToModuleGroupType(Constant.UI_MODULE_GROUP_NAME);

        protected override void OnAdd(GUnit unit)
        {
            var unitData = unit.GetData<UnitData>();
            var panelData = unit.GetData<PanelData>();
            var panel = unit.GetData<Panel>();
            panelData.stateTypeProperty.Subscribe(state => 
            {
                if (state == PanelStateType.Show)
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
            panel.UITweenProcessor.PlayOpen();
        }

        void Hide(GUnit unit, Panel panel)
        {
            panel.UITweenProcessor.PlayClose(() => 
            {
                OnHide(unit, panel);
                panel.gameObject.SetActive(false);
            });
        }
    }
}