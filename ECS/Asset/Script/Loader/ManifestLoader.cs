namespace Asset
{
    using System;
    using UniRx;
    using UnityEngine;
    using UnityEngine.Networking;
    using System.Linq;
    using System.IO;
    using ECS;
    using ECS.Data;
    using ECS.Helper;
    using ECS.Common;

    internal enum ManifestState
    {
        NotLoaded,
        Loading,
        Loaded
    }

    internal class ManifestLoader
    {
        public ManifestState ManifestState { get; set; } = ManifestState.NotLoaded;
        public AssetBundleManifest Manifest { get; set; }

        public IObservable<UnityWebRequest> SendManifestRequest(string url)
        {
            var version = PlayerPrefs.GetInt(AssetConstant.MANIFEST_VERSION_KEY, 0);
            var hash = AssetPath.Version2Hash(version);
            if (Caching.IsVersionCached(url, hash))
            {
                return UnityWebRequest.Head(url).SendAsObserable()
                    .ContinueWith(operation => 
                    {
                        var req = operation == null ? null : operation.webRequest;
                        if (req != null && req.GetResponseHeader(AssetConstant.HTTP_ETAG_FLAG) 
                            == PlayerPrefs.GetString(AssetConstant.MANIFEST_ETAG_KEY))
                        {
                            return UnityWebRequestAssetBundle.GetAssetBundle(url, hash)
                                .SendAsObserable().Select(_ => _.webRequest);
                        }
                        else
                        {
                            hash = AssetPath.Version2Hash(version + 1);
                            return UnityWebRequestAssetBundle.GetAssetBundle(url, hash)
                                .SendAsObserable().Select(_ => _.webRequest);
                        }
                    });
            }
            else
            {
                return UnityWebRequestAssetBundle.GetAssetBundle(url, hash)
                    .SendAsObserable().Select(_ => _.webRequest);
            }
        }

        public IObservable<Unit> LoadManifest() 
        {
            ManifestState = ManifestState.Loading;

            var assetCoreUnit = WorldManager.Instance.Unit.GetUnit(AssetConstant.ASSET_CORE_UNIT_NAME);
            var processData = assetCoreUnit.GetData<AssetProcessData>();
            var manifestUrl = Path.Combine(processData.CDN, AssetConstant.MANIFEST_BUNDLE_NAME);

            AssetBundle mainBundle = null;
            return SendManifestRequest(manifestUrl).LoadAssetBundle()
            .ContinueWith(bundle =>
            {
                mainBundle = bundle;
                return bundle.LoadAsObserable<AssetBundleManifest>(AssetConstant.MANIFEST_NAME);
            })
            .Select(request =>
            {
                Manifest = request.asset as AssetBundleManifest;
                ManifestState = ManifestState.Loaded;
                return Unit.Default;
            })
            .DoOnCompleted(() => mainBundle.Unload(false))
            .DoOnError(exp => Log.I("Load manifest failed {0} {1}!", exp.ToString(), exp.Message));
        }
    }
}