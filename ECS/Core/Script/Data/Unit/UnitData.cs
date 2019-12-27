namespace ECS.Data
{
    using UniRx;
    using ECS.Common;

    public enum UnitStateType
    {
        None = 0,
        Init,
    }
    
    public class UnitData : IData, IPoolObject
    {
        public string tag;
        public int unitType;
        public IReactiveProperty<UnitStateType> stateTypeProperty = new ReactiveProperty<UnitStateType>(UnitStateType.None);
        public CompositeDisposable disposable;

        public bool IsInUse { get; set; }
        public void Clear()
        {
            tag = string.Empty;
            unitType = 0;
            stateTypeProperty.Value = UnitStateType.None;
        }
    }
}