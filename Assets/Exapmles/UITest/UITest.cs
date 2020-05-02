using ECS;
using ECS.Module;
using Game.UI.Module;

public class UITest : GameStart
{
    protected override void StartGame() 
    {
        UIProcess.Show("Prefabs/UI/UITestPanel", false, null);
    }

    protected override void RegisterGameModule()
    {
        WorldManager.Instance.Module.Register(new DefaultUIModule());
        WorldManager.Instance.Module.Register(new SimpleTipsPanel());
        WorldManager.Instance.Module.Register(new UITestPanel());
    }
}