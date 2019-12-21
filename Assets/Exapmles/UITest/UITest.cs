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
        UIManager.Show("Prefabs/UI/UITestPanel", false, null).Subscribe();
    }

    protected override void RegisterGameModule()
    {
        WorldManager.Instance.Module.Register(new SimpleTipsPanel());
        WorldManager.Instance.Module.Register(new UITestPanel());
        WorldManager.Instance.Module.Register(new UITweenTriggerTestPanel());
        WorldManager.Instance.Module.Register(new UIAnimationTriggerTestPanel());
    }
}