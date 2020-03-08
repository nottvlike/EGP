using ECS;
using ECS.UI;
using ECS.UI.Module;
using UniRx;
using Game.UI;

public class UIPreloadTest : GameStart
{
    protected override void StartGame() 
    {
        UIManager.Show("Prefabs/UI/UIPreloadTestPanel", false, null);
    }

    protected override void RegisterGameModule()
    {
        WorldManager.Instance.Module.Register(new DefaultUIModule());
        WorldManager.Instance.Module.Register(new UIPreloadTestPanel());
    }
}