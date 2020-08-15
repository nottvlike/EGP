namespace Asset
{
    using ECS.Common;
    using ECS.Helper;
    using UObject = UnityEngine.Object;
    using System;
    using UniRx;
    using UnityEngine;
    using UnityEngine.Networking;
    using System.Collections.Generic;
    using System.Linq;
    using System.IO;

    internal class AssetBundleLoader
    {
        AssetBundleManifest _manifest;
        string _cdn;

        public AssetBundleLoader(AssetBundleManifest manifest, string cdn)
        {
            _manifest = manifest;
            _cdn = cdn;
        }

        Dictionary<string, AssetBundleCache> _assetBundleCacheDict = new Dictionary<string, AssetBundleCache>();
        Dictionary<string, LoadingCache<AssetBundleCache>> _loadingCacheDict = new Dictionary<string, LoadingCache<AssetBundleCache>>();

        IObservable<AssetBundleCache> GetAssetBundleCache(string bundleName)
        {
            return Observable.Defer(() =>
            {
                AssetBundleCache assetBundleCache;
                if (_assetBundleCacheDict.TryGetValue(bundleName, out assetBundleCache))
                {
                    return Observable.Return<AssetBundleCache>(assetBundleCache);
                }
                
                LoadingCache<AssetBundleCache> loadingCache = null;
                if (!_loadingCacheDict.TryGetValue(bundleName, out loadingCache))
                {
                    loadingCache = new LoadingCache<AssetBundleCache>();
                    _loadingCacheDict.Add(bundleName, loadingCache);

                    var url = Path.Combine(_cdn, bundleName);
                    var hash = _manifest.GetAssetBundleHash(bundleName);
                    var request = hash.isValid ? UnityWebRequestAssetBundle.GetAssetBundle(url, hash, 0) : 
                        UnityWebRequestAssetBundle.GetAssetBundle(url);
                    request.SendAsObserable()
                        .Select(operation => operation.webRequest).LoadAssetBundle()
                        .Select(bundle => new AssetBundleCache(bundleName, bundle))
                        .Do(bundleCache => 
                        {
                            _assetBundleCacheDict.Add(bundleName, bundleCache);
                            _loadingCacheDict.Remove(bundleName);
                        })
                        .Subscribe(loadingCache);
                }

                return loadingCache.ToLoadingObserable();
            });
        }

        IObservable<AssetBundleCache> GetAssetBundleCacheWithDependencies(string bundleName)
        {
            var dependenciesList = _manifest.GetAllDependencies(bundleName);
            var cancel = new CompositeDisposable();

            AssetBundleCache mainAssetBundleCache = null;
            return GetAssetBundleCache(bundleName)
            .Do(assetBundleCache => 
            {
                mainAssetBundleCache = assetBundleCache;
                cancel.Add(mainAssetBundleCache.Reference());
            }).AsEnumerable()
            .Concat(dependenciesList.Select(dependencyName => GetAssetBundleCache(dependencyName)
                .Do(dependencyCache => cancel.Add(dependencyCache.Reference()))))
            .Merge(5).AsSingleUnitObservable()
            .Where(_ => 
            {
                if (mainAssetBundleCache != null)
                    return true;
                
                cancel.Dispose();
                return false;
            })
            .Select(_ => mainAssetBundleCache)
            .DoOnCancel(() => cancel.Dispose())
            .DoOnError(ex => cancel.Dispose());
        }

        IObservable<Unit> WhenCacheReady()
        {
            if (Caching.ready) return Observable.ReturnUnit();
            return Observable.Defer(() =>
            {
                if (Caching.ready) return Observable.ReturnUnit();
                return Observable.EveryUpdate().AsUnitObservable().Where(_ => Caching.ready)
                    .FirstOrDefault();
            });
        }

        public IObservable<UObject> Load(string bundleName, string assetName)
        {
            return WhenCacheReady().ContinueWith(_ => GetAssetBundleCacheWithDependencies(bundleName))
                .ContinueWith(assetBundleCache => assetBundleCache.Load(assetName));
        }

        public void Clear(string bundleName)
        {
            var assetBundleCache = _assetBundleCacheDict[bundleName];
            assetBundleCache.Unreference();
            if (assetBundleCache.BundleReference == 0)
            {
                assetBundleCache.Unload(true);
                _assetBundleCacheDict.Remove(bundleName);
            }

            var dependenciesList = _manifest.GetAllDependencies(bundleName);
            foreach(var dependency in dependenciesList)
            {
                assetBundleCache = _assetBundleCacheDict[dependency];
                assetBundleCache.Unreference();
                if (assetBundleCache.BundleReference == 0)
                {
                    assetBundleCache.Unload(true);
                    _assetBundleCacheDict.Remove(dependency);
                }
            }
        }
    }
}