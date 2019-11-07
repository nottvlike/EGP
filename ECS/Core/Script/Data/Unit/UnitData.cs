namespace ECS.Data
{
    using UniRx;
    using Game.Tool;

    public enum UnitStateType
    {
        None = 0,
        Init,
    }
    
    public class UnitData : IData, IPoolObject
    {
        public string tag;
        public int unitType;
        public int requiredModuleGroup;
        public IReactiveProperty<UnitStateType> stateTypeProperty;
        public CompositeDisposable disposable;

        public bool IsInUse { get; set; }
        public void Clear()
        {
            tag = string.Empty;
            unitType = -1;
            requiredModuleGroup = -1;
            stateTypeProperty.Value = UnitStateType.None;
            disposable = null;
        }
    }
}