﻿namespace ECS.Module
{
    using System.Collections.Generic;
    using ECS.Common;
    using ECS.Config;
    using ECS.Data;
    using GUnit = ECS.Unit.Unit;
    using UniRx;
    using System.Linq;
    
    public sealed partial class ModuleManager
    {
        public IReadOnlyList<IModule> ModuleList => _moduleList;
        List<IModule> _moduleList = new List<IModule>();

        LayerMaskConfig _moduleGroupTypeConfig;

        bool _isInited;

        public int TagToModuleGroupType(string tag)
        {
            return _moduleGroupTypeConfig.TagToLayer(tag);
        }

        public string ModuleGroupTypeToTag(int unitGroupType)
        {
            return _moduleGroupTypeConfig.LayerToTag(unitGroupType);
        }

        public void Init(LayerMaskConfig moduleGroupTypeConfig)
        {
            if (_isInited)
            {
                return;
            }

            _moduleGroupTypeConfig = moduleGroupTypeConfig;

            _isInited = true;
            RegisterCoreModule();
        }

        public void Register(IModule module)
        {
#if DEBUG
            if (module.RequiredDataList.Length <= 0 || module.Group == 0)
            {
                Log.W("Module {0} has no need to add!", module.GetType());
                return;
            }

            if (_moduleList.IndexOf(module) != -1)
            {
                Log.W("Module {0} has been registered!", module.GetType());
                return;
            }
#endif

            _moduleList.Add(module);
        }

        void RegisterCoreModule()
        {
            Register(new GameSystem());
            Register(new GameState());
        }

        internal void UpdateMeetModuleList(GUnit unit)
        {
            var moduleList = _moduleList.Where(_ => ((int)_.Group & unit.RequiredModuleGroup) != 0);
            foreach (var module in moduleList)
            {
                var isMeet = module.IsMeet(unit.GetAllData());
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