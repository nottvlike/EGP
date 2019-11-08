using ECS;
using ECS.Common;
using ECS.Data;
using ECS.Module;
using ECS.Factory;
using EUnit = ECS.Entity.Unit;
using UniRx;
using System;

public class HelloWorld : GameStart
{
    protected override void StartGame() 
    {
        WorldManager.Instance.Factory.CreateHelloWorld();
    }

    protected override void RegisterGameModule()
    {
        WorldManager.Instance.Module.Register(new HelloWorldModule());
    }
}

public class HelloWorldData : IData
{
    public string helloWorld;
}

public class HelloWorldModule : Module
{
    public override int Group { get; protected set; } 
            = WorldManager.Instance.Module.TagToModuleGroupType(GameConstant.HELLO_WORLD_GROUP_TYPE);

    public HelloWorldModule()
    {
        RequiredDataList = new Type[]
        {
            typeof(HelloWorldData)
        };
    }

    protected override void OnAdd(EUnit unit)
    {
        var unitData = unit.GetData<UnitData>();
        var helloWorldData = unit.GetData<HelloWorldData>();

        Observable.Interval(TimeSpan.FromSeconds(1)).Subscribe(_ =>
        {
            Log.I(helloWorldData.helloWorld);
        }).AddTo(unitData.disposable);
    }
}

public static class HelloWorldFactory
{
    public static void CreateHelloWorld(this UnitFactory factory)
    {
        var unit = factory.CreateUnit();

        var helloWorldData = new HelloWorldData();
        helloWorldData.helloWorld = "Hello World!";
        unit.AddData(helloWorldData);

        var unitData = unit.GetData<UnitData>();
        unitData.requiredModuleGroup = WorldManager.Instance.Module
            .TagToModuleGroupType(GameConstant.HELLO_WORLD_GROUP_TYPE);
        unitData.stateTypeProperty.Value = UnitStateType.Init;
    }
}