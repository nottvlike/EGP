#if UNITY_EDITOR

namespace ECS.Helper
{
    using UnityEditor;
    using UnityEngine;
    using UObject = UnityEngine.Object;
    using System;
    using UniRx;
    using ECS;
    using ECS.Config;
    using ECS.Common;
    
    public class SimulateAssetLoader : IAssetLoader
    {
        AssetConfig _assetConfig;

        public AssetBundleManifest Manifest => null;

        public IObservable<Unit> Init()
        {
            return LoadAssetConfig().Do(assetConfig =>
            {
                _assetConfig = assetConfig;
            }).AsUnitObservable();
        }

        IObservable<AssetConfig> LoadAssetConfig()
        {
            var assetFullPath = AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(
                Constant.ASSET_CONFIG_BUNDLE_NAME, Constant.ASSET_CONFIG_ASSET_NAME);
            return Observable.Return(AssetDatabase.LoadAssetAtPath<AssetConfig>(assetFullPath[0]),
                    Scheduler.MainThreadIgnoreTimeScale);
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
                return Observable.Return(AssetDatabase.LoadAssetAtPath<T>(
                    assetInfo.fullPath), Scheduler.MainThreadIgnoreTimeScale);
            }
            else
            {
                var assetFullPath = AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(
                    assetInfo.bundleName, assetInfo.assetName);
                return Observable.Return(AssetDatabase.LoadAssetAtPath<T>(assetFullPath[0]),
                    Scheduler.MainThreadIgnoreTimeScale);
            }
        }

        public void Clear(string assetPath)
        {
        }
    }
}

#endif