using System.Collections.Generic;
using System.Linq;
using System.IO;
using System;
using UnityEngine;
using UnityEngine.Networking;
using UniRx;
using ECS;
using ECS.Data;
using ECS.Common;
using ECS.Helper;

public class CustomDownloadHandler : DownloadHandler<AssetBundle>
{
    public CustomDownloadHandler(string name, string url) : base(name, url) {}

    byte[] assetBundleData;

    public override IObservable<Unit> Download()
    {
        return UnityWebRequest.Get(Url)
            .SendWebRequest().AsAsyncOperationObservable().Where(_ => 
            {
                var req = _.webRequest;
                if (req.isHttpError || req.isNetworkError)
                {
                    throw new Exception(string.Format("Download {0} failed, {1}!", Url, req.error));
                }

                return true;
            })
            .Select(_ => _.webRequest)
            .Do(req => 
            {
                IsDownload = true;
                assetBundleData = req.downloadHandler.data;
                downloadAsset = AssetBundle.LoadFromMemory(req.downloadHandler.data);
                Save();
            }).AsUnitObservable();
    }

    public override void Save() 
    {
        var assetBundleDir = Path.Combine(Application.persistentDataPath, "AssetBundles");
        if (!Directory.Exists(assetBundleDir))
        {
            Directory.CreateDirectory(assetBundleDir);
        }

        File.WriteAllBytes(Path.Combine(assetBundleDir, Name), assetBundleData);
    }
}

public class CustomDownloadTest : GameStart
{
    string[] assetBundleList = new string[]
    {
        "AssetBundles",
        "prefabs_actor_cube1",
        "prefabs_actor_plane1",
        "prefabs_actor_sphere1",
        "prefabs_ui"
    };

    List<CustomDownloadHandler> assetDownloadHandlerList = new List<CustomDownloadHandler>();

    protected override void StartGame() 
    {
        var assetCoreUnit = WorldManager.Instance.Unit.GetUnit(AssetConstant.ASSET_CORE_UNIT_NAME);
        var processData = assetCoreUnit.GetData<AssetProcessData>();

        var assetDownList = assetBundleList.Where(_ => !ContainsAssetName(_));
        foreach (var assetName in assetDownList)
        {
            var assetUrl = Path.Combine(processData.CDN, assetName);
            assetDownloadHandlerList.Add(new CustomDownloadHandler(assetName, assetUrl));
        }

        assetDownloadHandlerList
            .Where(handler => !handler.IsFetchHead).Select(handler => handler.FetchHead().Do(_ =>
            {
                Log.I("fetch head {0} size {1}", handler.Name, handler.Size);
            }))
            .Merge().ContinueWith(_ => 
            {

                return assetDownloadHandlerList.Where(handler => !handler.IsDownload)
                    .Select(handler => handler.Download().ContinueWith(unit => 
                    {
                        handler.Save();
                        return handler.GetAsset().Do(assetBundle => 
                        {
                            Log.I("downloaded {0}!", handler.Name);
                            
                            if (assetBundle != null)
                            {
                                var assetNameList = assetBundle.GetAllAssetNames();
                                foreach (var assetName in assetNameList)
                                {
                                    Log.I("asset name " + assetName);
                                }
                            }
                        });
                    })).Merge();
            }).Subscribe();
    }

    bool ContainsAssetName(string assetName)
    {
        foreach (var assetDownloadHandler in assetDownloadHandlerList)
        {
            if (assetDownloadHandler.Name == assetName)
            {
                return true;
            }
        }

        return false;
    }

    protected override void RegisterGameModule()
    {
    }
}
