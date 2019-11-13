using ECS;
using ECS.Common;
using ECS.Data;
using ECS.Module;
using ECS.Factory;
using ECS.UI;
using EUnit = ECS.Entity.Unit;
using UniRx;
using System;
using Game.UI;

public class UITest : GameStart
{
    protected override void StartGame() 
    {
        Observable.Timer(TimeSpan.FromSeconds(3)).Subscribe(time =>
        {
            UIManager.Show("Prefabs/UI/SimpleTipsPanel", false, "This is a test!").Subscribe();
            Observable.Timer(TimeSpan.FromSeconds(6)).Subscribe(_ =>
            {
                UIManager.Hide("Prefabs/UI/SimpleTipsPanel").Subscribe();
            });
        });
    }

    protected override void RegisterGameModule()
    {
        WorldManager.Instance.Module.Register(new SimpleTipsPanel());
    }
}