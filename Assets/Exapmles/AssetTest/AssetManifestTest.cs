using ECS.Common;
using Asset;
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
        var manifest = AssetManager.Manifest;
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