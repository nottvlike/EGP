namespace ECS.Data
{
    using UniRx;
    using ECS.Common;

    public enum UnitStateType
    {
        None = 0,
        Init,
    }
    
    public class UnitData : IData
    {
        public string tag;
        public int unitType;
        public int requiredModuleGroup;
        public IReactiveProperty<UnitStateType> stateTypeProperty;
        public CompositeDisposable disposable;
    }
}