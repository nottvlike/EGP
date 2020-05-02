using ECS;
using ECS.Data;
using ECS.Common;
using UniRx;

public class AssetManifestTest : GameStart
{
    CompositeDisposable disposables = new CompositeDisposable();

    protected override void Awake()
    {
        LoadGame(false);
    }

    protected override void StartGame() 
    {
        var assetCoreUnit = WorldManager.Instance.Unit.GetUnit(AssetConstant.ASSET_CORE_UNIT_NAME);
        var processData = assetCoreUnit.GetData<AssetProcessData>();

        var manifest = processData.Manifest;
        var assetBundleNameList = manifest.GetAllAssetBundles();
        foreach (var assetBundleName in assetBundleNameList)
        {
            Log.I("asset bundle {0}", assetBundleName);
        }
    }

    protected override void RegisterGameModule()
    {
    }
}