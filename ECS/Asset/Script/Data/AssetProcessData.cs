namespace ECS.Data
{
    using UnityEngine;
    using UObject = UnityEngine.Object;
    using Asset;
    using System;

    internal struct LoadedAssetInfo
    {
        public UObject asset;
        public UObject instance;
    }

    public class AssetProcessData : IData
    {
        public string CDN;
        public AssetBundleManifest Manifest;
    }
}