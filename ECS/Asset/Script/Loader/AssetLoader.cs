namespace ECS.Helper
{
    using UniRx;
    using System;
    using UObject = UnityEngine.Object;
    using UnityEngine;
    using ECS;
    using ECS.Data;
    using ECS.Common;
    using ECS.Config;
    using Asset;

    internal class AssetLoader : IAssetLoader
    {
        AssetConfig _assetConfig;
        
        ManifestLoader _manifestLoader = new ManifestLoader();
        AssetBundleLoader _assetBundleLoader;

        public AssetBundleManifest Manifest => _manifestLoader.Manifest;
        
        public IObservable<Unit> Init()
        {
            return  CachingIsReady().ContinueWith(_ =>
            {
                return _manifestLoader.LoadManifest();
            }).ContinueWith(_ =>
            {
                return LoadAssetConfig().Do(assetConfig =>
                {
                    _assetConfig = assetConfig;
                }).AsUnitObservable();
            });
        }

        IObservable<AssetConfig> LoadAssetConfig()
        {
            if (_assetBundleLoader == null)
            {
                var assetCoreUnit = WorldManager.Instance.Unit.GetUnit(AssetConstant.ASSET_CORE_UNIT_NAME);
                var assetCoreData = assetCoreUnit.GetData<AssetProcessData>();

                _assetBundleLoader = new AssetBundleLoader(_manifestLoader.Manifest, assetCoreData.CDN);
            }

            return _assetBundleLoader.Load(AssetConstant.ASSET_CONFIG_BUNDLE_NAME,
                AssetConstant.ASSET_CONFIG_ASSET_NAME).Select(obj => (AssetConfig)obj);
        }

        public IObservable<T> Load<T>(string assetPath) where T : UObject
        {
            if (!_assetConfig.Contains(assetPath))
            {
                Log.I("Asset {0} not found in assetconfig ,refresh asset config first!", assetPath);
                return Observable.Return<T>(null);
            }

            var assetInfo = _assetConfig.GetAssetInfo(assetPath);
            if (!assetInfo.isFromBundle)
            {
                return Resources.LoadAsync(assetPath).AsAsyncOperationObservable().Select(obj => (T)obj.asset);
            }
            else
            {
                return _assetBundleLoader.Load(assetInfo.bundleName, assetInfo.assetName).Select(obj => (T)obj);
            }
        }

        public void Clear(string assetPath)
        {
            var assetInfo = _assetConfig.GetAssetInfo(assetPath);
            _assetBundleLoader.Clear(assetInfo.bundleName);
        }

        public IObservable<Unit> CachingIsReady()
        {
            if (!Caching.ready)
            {
                return Observable.Interval(TimeSpan.FromMilliseconds(100)).Where(_ => Caching.ready).First()
                    .AsUnitObservable();
            }
            else
            {
                return Observable.ReturnUnit();
            }
        }
    }
}