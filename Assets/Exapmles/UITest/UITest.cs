using ECS;
using ECS.UI;
using ECS.UI.Module;
using UniRx;
using Game.UI;

public class UITest : GameStart
{
    protected override void StartGame() 
    {
        UIManager.Show("Prefabs/UI/UITestPanel", false, null);
    }

    protected override void RegisterGameModule()
    {
        WorldManager.Instance.Module.Register(new DefaultUIModule());
        WorldManager.Instance.Module.Register(new SimpleTipsPanel());
        WorldManager.Instance.Module.Register(new UITestPanel());
    }
}