using ECS;
using ECS.Common;
using ECS.Data;
using ECS.Module;
using ECS.Factory;
using ECS.UI;
using EUnit = ECS.Unit.Unit;
using UniRx;
using System;
using Game.UI;

public class UITest : GameStart
{
    protected override void StartGame() 
    {
        UIManager.Show("Prefabs/UI/UITestPanel", false, null).Subscribe();
    }

    protected override void RegisterGameModule()
    {
        WorldManager.Instance.Module.Register(new DefaultUIModule());
        WorldManager.Instance.Module.Register(new SimpleTipsPanel());
        WorldManager.Instance.Module.Register(new UITestPanel());
    }
}