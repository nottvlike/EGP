namespace ECS.Factory
{
    using UniRx;
    using GUnit = ECS.Entity.Unit;
    using Data;
    using System.Linq;
    using ECS;
    using ECS.Common;
    using ECS.Entity;

    public sealed class UnitFactory
    {
        public void DestroyUnit(GUnit unit)
        {
            var unitData = unit.GetData<UnitData>();
            
            unitData.disposable?.Dispose();
            unitData.disposable = null;

            if (!string.IsNullOrEmpty(unitData.tag))
            {
                WorldManager.Instance.Unit.ClearCache(unitData.tag);
            }

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

                    UpdateMeetModuleList(unit);
                }
            }).AddTo(unitData.disposable);

            unit.AddDataSubject.Merge(unit.RemoveDataSubject)
                .Subscribe(_ => UpdateMeetModuleList(unit)).AddTo(unitData.disposable);
            return unit;
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