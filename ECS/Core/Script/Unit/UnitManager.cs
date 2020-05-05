namespace ECS.Unit
{
    using ECS.Data;
    using ECS.Common;
    using ECS.Config;
    using System.Collections.Generic;

    public sealed partial class UnitManager
    {
        WorldManager worldMgr => WorldManager.Instance;

        LayerConfig _unitTypeConfig;

        public void Init(LayerConfig unitTypeConfig)
        {
            _unitTypeConfig = unitTypeConfig;

            InitGameCore();
        }

        public int TagToUnitType(string tag)
        {
            return _unitTypeConfig.TagToLayer(tag);
        }

        public string UnitTypeToTag(int unitType)
        {
            return _unitTypeConfig.LayerToTag(unitType);
        }

        void InitGameCore()
        {
            var requiredModuleGroup = worldMgr.Module.TagToModuleGroupType(Constant.SYSTEM_MODULE_GROUP_NAME);
            var gameCore = worldMgr.Factory.CreateUnit(requiredModuleGroup);
            gameCore.AddData(new SystemData());

            gameCore.AddData(new UpdateData());
            gameCore.AddData(new LateUpdateData());
            gameCore.AddData(new FixedUpdateData());

            gameCore.AddData(new GameStateData());

            var unitData = gameCore.GetData<UnitData>();
            unitData.unitType = TagToUnitType(Constant.SYSTEM_UNIT_TYPE_NAME);
            unitData.tag = Constant.GAME_CORE_UNIT_NAME;
            
            unitData.stateTypeProperty.Value = UnitStateType.Init;
        }

        #region unit dict

        public IReadOnlyDictionary<uint, Unit> UnitDIctionary => _unitDictionary;
        
        Dictionary<uint, Unit> _unitDictionary = new Dictionary<uint, Unit>();

        public Unit GetUnit(uint unitId)
        {
#if DEBUG
            if (!_unitDictionary.ContainsKey(unitId))
            {
                Log.E("GetUnit failed, cannot find unit {0}", unitId);
            }
#endif

            return _unitDictionary[unitId];
        }

        public T GetData<T>(uint unitId) where T : class, IData
        {
#if DEBUG
            if (!_unitDictionary.ContainsKey(unitId))
            {
                Log.E("GetData failed, cannot find unit {0}", unitId);
                return null;
            }
#endif
            return WorldManager.Instance.Data.GetData(unitId, typeof(T)) as T;
        }

        internal void AddUnit(uint unitId, Unit unit, bool replace = false)
        {
#if DEBUG
            if (_unitDictionary.ContainsKey(unitId) && !replace)
            {
                Log.E("AddUnit failed, unit {0} has been added!", unitId);
                return;
            }
#endif

            _unitDictionary[unitId] = unit;
        }

        internal void RemoveUnit(uint unitId)
        {
#if DEBUG
            if (!_unitDictionary.ContainsKey(unitId))
            {
                Log.E("RemoveUnit failed, unit {0} not exist!", unitId);
                return;
            }
#endif

            _unitDictionary.Remove(unitId);
        }

        #endregion

        #region unit cache

        Dictionary<string, Unit> _unitCacheDictionary = new Dictionary<string, Unit>();

        public Unit GetUnit(string tag)
        {
#if DEBUG
            if (!_unitCacheDictionary.ContainsKey(tag))
            {
                Log.E("GetUnit failed, unit named {0} doesn't cached!", tag);
            }
#endif

            return _unitCacheDictionary[tag];
        }

        public T GetData<T>(string tag) where T : class, IData
        {
#if DEBUG
            if (!_unitCacheDictionary.ContainsKey(tag))
            {
                Log.E("GetData failed, unit named {0} doesn't cached!", tag);
                return null;
            }
#endif

            return GetData<T>(_unitCacheDictionary[tag].UnitId);
        }

        internal void ClearCache(string tag)
        {
#if DEBUG
            if (!_unitCacheDictionary.ContainsKey(tag))
            {
                Log.E("ClearCache failed, unit named {0} doesn't cached!", tag);
                return;
            }
#endif

            _unitCacheDictionary.Remove(tag);
        }

        internal void AddCache(string tag, Unit unit)
        {
#if DEBUG
            if (_unitCacheDictionary.ContainsKey(tag))
            {
                Log.E("AddCache failed, unit named {0} has been cached!", tag);
                return;
            }
#endif

            _unitCacheDictionary[tag] = unit;
        }

        #endregion
    }
}