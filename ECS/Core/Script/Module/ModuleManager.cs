namespace ECS.Module
{
    using System.Collections.Generic;
    using ECS.Common;
    using ECS.Config;

    public sealed partial class ModuleManager
    {
        public IReadOnlyList<Module> ModuleList => _moduleList;
        List<Module> _moduleList = new List<Module>();

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

        public void Register(Module module)
        {
#if DEBUG
            if (module.RequiredDataList.Length <= 0)
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
        }
    }   
}