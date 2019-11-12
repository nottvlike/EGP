using ECS.Common;
using ECS;
using ECS.Config;
using ECS.UI;
using ECS.Module;
using Asset;
using UnityEngine;
using UniRx;
using System.Threading.Tasks;
using System;

public abstract class GameStart : MonoBehaviour
{
    void Awake() 
    {
        LoadGame();
    }

    protected async void LoadGame()
    {
        var moduleGroupTypeConfigPath = AssetPath.GetAssetPathFromResourcePath(AssetConstant.MODULE_GROUP_TYPE_CONFIG_PATH);
        var moduleGroupTypeConfig = Resources.Load<LayerMaskConfig>(moduleGroupTypeConfigPath);
        
        var worldMgr = WorldManager.Instance;
        worldMgr.Module.Init(moduleGroupTypeConfig);
        
        var unitTypeConfigPath = AssetPath.GetAssetPathFromResourcePath(AssetConstant.UNIT_TYPE_CONFIG_PATH);
        var unitTypeConfig = Resources.Load<LayerConfig>(unitTypeConfigPath);

        worldMgr.Unit.Init(unitTypeConfig);

#if UNITY_EDITOR
        AssetManager.IsSimulate = true;
#endif
        AssetManager.CDN = "http://localhost:8000/AssetBundles";

        var assetPath = AssetPath.GetAssetPathFromResourcePath(AssetConstant.ASSET_CONFIG_PATH);
        var assetCofig = Resources.Load<AssetConfig>(assetPath);
        await AssetManager.Init(assetCofig);

        UIManager.Init();

        RegisterGameModule();
        
        StartGame();
    }

    protected virtual void StartGame() {}
    protected virtual void RegisterGameModule() {}
}
