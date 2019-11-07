namespace ECS
{
    using ECS.Common;
    using ECS.Config;
    using ECS.Module;
    using ECS.Entity;
    using ECS.Factory;

    public sealed class WorldManager : Singleton<WorldManager>
    {
        public ModuleManager Module { get; private set; } = new ModuleManager();
        
        public ConfigManager Config { get; private set; } = new ConfigManager();

        public UnitManager Unit { get; private set; } = new UnitManager();

        public UnitFactory Factory { get; private set; } = new UnitFactory();
    }

}