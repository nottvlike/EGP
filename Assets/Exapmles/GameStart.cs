using ECS;
using ECS.Config;
using ECS.UI;
using Asset;
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

        IAssetLoader customLoader = null;
#if UNITY_EDITOR
        if (isSimulate)
        {
            customLoader = new AssetEditor.SimulateAssetLoader();
        }
#endif

        AssetManager.CDN = "http://localhost:8000/AssetBundles";
        await AssetManager.Init(customLoader);

        UIManager.Init();

        RegisterGameModule();
        
        StartGame();
    }

    protected virtual void StartGame() {}
    protected virtual void RegisterGameModule() {}
}
