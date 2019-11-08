using UnityEngine;
using ECS;
using ECS.Config;

public abstract class GameStart : MonoBehaviour
{
    void Awake() 
    {
        LoadGame();
    }

    protected void LoadGame()
    {
        var moduleGroupTypeConfig = Resources.Load<LayerMaskConfig>("Config/ModuleGroupTypeConfig");
        var worldMgr = WorldManager.Instance;
        worldMgr.Module.Init(moduleGroupTypeConfig);

        var unitTypeConfig = Resources.Load<LayerConfig>("Config/UnitTypeConfig");
        worldMgr.Unit.Init(unitTypeConfig);

        RegisterGameModule();
        
        StartGame();
    }

    protected virtual void StartGame() {}
    protected virtual void RegisterGameModule() {}
}
