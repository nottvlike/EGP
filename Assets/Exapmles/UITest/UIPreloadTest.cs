using ECS;
using ECS.UI;
using UniRx;
using Game.UI;

public class UIPreloadTest : GameStart
{
    protected override void StartGame() 
    {
        UIManager.Show("Prefabs/UI/UIPreloadTestPanel", false, null).Subscribe();
    }

    protected override void RegisterGameModule()
    {
        WorldManager.Instance.Module.Register(new DefaultUIModule());
        WorldManager.Instance.Module.Register(new UIPreloadTestPanel());
    }
}