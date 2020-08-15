namespace Asset
{
    using System;
    using UniRx;
    using UnityEngine;

    internal static class AssetBundleExtension
    {
        public static IObservable<AssetBundleRequest> LoadAllAsObserable(this AssetBundle bundle)
        {
            if (bundle == null) return Observable.Empty<AssetBundleRequest>();
            return bundle.LoadAllAssetsAsync().AsAsyncOperationObservable();
        }

        public static IObservable<AssetBundleRequest> LoadAsObserable<T>(this AssetBundle bundle, string assetName)
        {
            if (bundle == null) return Observable.Empty<AssetBundleRequest>();
            return bundle.LoadAssetAsync<T>(assetName).AsAsyncOperationObservable();
        }
    }
}