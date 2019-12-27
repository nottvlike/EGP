namespace ECS
{
    using ECS.Common;
    using ECS.Config;
    using ECS.Module;
    using ECS.Unit;
    using ECS.Factory;
    using ECS.Data;

    public sealed class WorldManager : Singleton<WorldManager>
    {
        internal DataManager Data { get; private set; } = new DataManager();

        public ModuleManager Module { get; private set; } = new ModuleManager();
        
        public ConfigManager Config { get; private set; } = new ConfigManager();

        public UnitManager Unit { get; private set; } = new UnitManager();

        public UnitFactory Factory { get; private set; } = new UnitFactory();
    }

}