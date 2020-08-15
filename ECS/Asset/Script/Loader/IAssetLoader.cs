namespace ECS.Helper
{
    using UnityEngine;
    using UObject = UnityEngine.Object;
    using System;
    using UniRx;

    public interface IAssetLoader
    {
        AssetBundleManifest Manifest { get; }
        
        IObservable<Unit> Init();
        IObservable<T> Load<T>(string assetPath) where T : UObject;

        void Clear(string assetPath);
    }
}