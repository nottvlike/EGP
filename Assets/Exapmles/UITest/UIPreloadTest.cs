using ECS;
using ECS.Module;
using Game.UI.Module;
using Game.UI;

public class UIPreloadTest : GameStart
{
    protected override void StartGame() 
    {
        UIProcess.Show("Prefabs/UI/UIPreloadTestPanel", false, null);
    }

    protected override void RegisterGameModule()
    {
        WorldManager.Instance.Module.Register(new DefaultUIModule());
        WorldManager.Instance.Module.Register(new UIPreloadTestPanel());
    }
}