using ECS;
using ECS.Module;
using ECS.Config;
using ECS.Helper;
using ECS.Factory;
using UnityEngine;
using UniRx;

public abstract class GameStart : MonoBehaviour
{
    protected virtual void Awake() 
    {
        LoadGame();
    }

    protected async void LoadGame(bool isSimulate = true)
    {
        var moduleGroupTypeConfigPath = AssetPath.GetAssetPathFromResourcePath(AssetConstant.MODULE_GROUP_TYPE_CONFIG_PATH);
        var moduleGroupTypeConfig = Resources.Load<LayerMaskConfig>(moduleGroupTypeConfigPath);
        
        var worldMgr = WorldManager.Instance;
        worldMgr.Module.Init(moduleGroupTypeConfig);
        
        var unitTypeConfigPath = AssetPath.GetAssetPathFromResourcePath(AssetConstant.UNIT_TYPE_CONFIG_PATH);
        var unitTypeConfig = Resources.Load<LayerConfig>(unitTypeConfigPath);

        worldMgr.Unit.Init(unitTypeConfig);

        RegisterECSModule();

        IAssetLoader customLoader = null;
#if UNITY_EDITOR
        if (isSimulate)
        {
            customLoader = new SimulateAssetLoader();
        }
#endif
        worldMgr.Factory.CreateAssetProcess("http://localhost:8000/AssetBundles");
        await AssetProcess.Init(customLoader);

        worldMgr.Factory.CreateUIProcess();

        RegisterGameModule();
        
        StartGame();
    }

    void RegisterECSModule()
    {
        var module = WorldManager.Instance.Module;
        module.Register(new UIProcess());
        module.Register(new AssetProcess());
    }

    protected virtual void StartGame() {}
    protected virtual void RegisterGameModule() {}
}
