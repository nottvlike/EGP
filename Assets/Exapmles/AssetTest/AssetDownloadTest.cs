using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using Asset;
using UniRx;
using ECS.Common;

public class AssetDownloadTest : GameStart
{
    string[] assetBundleList = new string[]
    {
        "AssetBundles",
        "prefabs_actor_cube1",
        "prefabs_actor_plane1",
        "prefabs_actor_sphere1",
        "prefabs_ui"
    };

    List<AssetDownloadHandler> assetDownloadHandlerList = new List<AssetDownloadHandler>();

    protected override void StartGame() 
    {
        var assetDownList = assetBundleList.Where(_ => !ContainsAssetName(_));
        foreach (var assetName in assetDownList)
        {
            var assetUrl = Path.Combine(AssetManager.CDN, assetName);
            assetDownloadHandlerList.Add(new AssetDownloadHandler(assetName, assetUrl));
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
