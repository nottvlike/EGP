namespace ECS.Factory
{
    using UniRx;
    using GUnit = ECS.Unit.Unit;
    using ECS.Data;
    using ECS.Unit;
    using ECS;

    internal class Util
    {
        static uint uid = 1;
        public static uint GetUnionId()
        {
            return uid++;
        }
    }

    public sealed class UnitFactory
    {
        WorldManager worldMgr => WorldManager.Instance;
        DataManager dataMgr => worldMgr.Data;
        UnitManager unitMgr => worldMgr.Unit;

        public void DestroyUnit(GUnit unit)
        {
            var unitData = unit.GetData<UnitData>();
            
            unitData.disposable?.Dispose();
            unitData.disposable = null;

            dataMgr.ClearData(unit.UnitId);
            unit.UpdateMeetModuleList();

            if (!string.IsNullOrEmpty(unitData.tag))
            {
                unitMgr.ClearCache(unitData.tag);
            }
            
            dataMgr.ClearRemovedData(unit.UnitId);
            unitMgr.RemoveUnit(unit.UnitId);
        }

        public GUnit CreateUnit(int requiredModuleGroup, bool subscribleDestroy = true)
        {
            var unit = new GUnit(Util.GetUnionId(), requiredModuleGroup);
            unitMgr.AddUnit(unit.UnitId, unit);

            var unitData = unit.AddData<UnitData>();
            unitData.disposable = new CompositeDisposable();
            
            unitData.stateTypeProperty.Subscribe(_ => {
                if (_ == UnitStateType.Init)
                {
                    if (!string.IsNullOrEmpty(unitData.tag))
                    {
                        unitMgr.AddCache(unitData.tag, unit);
                    }

                    unit.UpdateMeetModuleList();
                }
            }).AddTo(unitData.disposable);

            if (subscribleDestroy)
            {
                unitData.stateTypeProperty.Subscribe(_ => {
                    if (_ == UnitStateType.Destroy)
                    {
                        DestroyUnit(unit);
                    }
                }).AddTo(unitData.disposable);
            }

            return unit;
        }
    } 
}