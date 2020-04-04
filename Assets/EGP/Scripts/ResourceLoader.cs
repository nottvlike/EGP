namespace Asset
{
    using UnityEngine;
    using UObject = UnityEngine.Object;
    using System;
    using UniRx;
    
    public class ResourceLoader : IAssetLoader
    {
        public AssetBundleManifest Manifest => null;

        public IObservable<Unit> Init()
        {
            return Observable.ReturnUnit();
        }

        public IObservable<T> Load<T>(string assetPath) where T : UObject
        {
            return Observable.Return(Resources.Load<T>(assetPath));
        }

        public void Clear(string assetPath)
        {
        }
    }
}