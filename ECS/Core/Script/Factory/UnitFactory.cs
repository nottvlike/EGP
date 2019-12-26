namespace ECS.Factory
{
    using UniRx;
    using GUnit = ECS.Unit.Unit;
    using Data;
    using System.Linq;
    using ECS;
    using ECS.Unit;

    public sealed class UnitFactory
    {
        public void DestroyUnit(GUnit unit)
        {
            var unitData = unit.GetData<UnitData>();
            
            unitData.disposable?.Dispose();
            unitData.disposable = null;

            ClearModuleList(unit);
            
            if (!string.IsNullOrEmpty(unitData.tag))
            {
                WorldManager.Instance.Unit.ClearCache(unitData.tag);
            }
            unitData.tag = string.Empty;
            unitData.unitType = 0;
            unitData.requiredModuleGroup = 0;
            unitData.stateTypeProperty.Value = UnitStateType.None;

            UnitPool.Release(unit);
        }

        public GUnit CreateUnit()
        {
            var unit = UnitPool.Get();

            var unitData = unit.GetData<UnitData>();
            unitData.disposable = new CompositeDisposable();
            
            unitData.stateTypeProperty.Subscribe(_ => {
                if (_ == UnitStateType.Init)
                {
                    if (!string.IsNullOrEmpty(unitData.tag))
                    {
                        WorldManager.Instance.Unit.AddCache(unitData.tag, unit);
                    }

                    unit.ObserverAddData.Concat(unit.ObserverRemoveData)
                        .Subscribe(data => UpdateMeetModuleList(unit)).AddTo(unitData.disposable);

                    UpdateMeetModuleList(unit);
                }
            }).AddTo(unitData.disposable);
            return unit;
        }

        static void ClearModuleList(GUnit unit)
        {
            var unitData = unit.GetData<UnitData>();
            var moduleList = WorldManager.Instance.Module.ModuleList.Where(_ => {
                return ((int)_.Group & unitData.requiredModuleGroup) != 0;
                });
            foreach (var module in moduleList)
            {
                if (module.Contains(unit.UnitId))
                {
                    module.Remove(unit);
                }
            }
        }

        static void UpdateMeetModuleList(GUnit unit)
        {
            var unitData = unit.GetData<UnitData>();
            var moduleList = WorldManager.Instance.Module.ModuleList.Where(_ => {
                return ((int)_.Group & unitData.requiredModuleGroup) != 0;
                });
            foreach (var module in moduleList)
            {
                var isMeet = module.IsMeet(unit.DataDictionary.Values);
                var isContains = module.Contains(unit.UnitId);

                if (!isContains && isMeet)
                {
                    module.Add(unit);
                }
                else if (isContains && !isMeet)
                {
                    module.Remove(unit);
                }
            }
        }
    } 
}