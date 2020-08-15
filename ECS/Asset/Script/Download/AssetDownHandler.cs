namespace ECS.Helper
{
    using UniRx;
    using System;
    using System.IO;
    using UnityEngine;
    using UnityEngine.Networking;
    using ECS;
    using ECS.Data;

    public sealed class AssetDownloadHandler : DownloadHandler<AssetBundle>
    {
        UnityWebRequest _request;
        static AssetBundleManifest _manifest;

        public bool IsCached
        {
            get
            {
                var assetHash = _manifest.GetAssetBundleHash(Name);
                return Caching.IsVersionCached(Url, assetHash);
            }
        }

        public AssetDownloadHandler(string url, string name) : base(url, name)
        {
            if (_manifest == null)
            {
                var assetCoreUnit = WorldManager.Instance.Unit.GetUnit(AssetConstant.ASSET_CORE_UNIT_NAME);
                var assetCoreData = assetCoreUnit.GetData<AssetProcessData>();
                _manifest = assetCoreData.Manifest;
            }
        }

        public override IObservable<Unit> Download()
        {
            var bundleName = Path.GetFileNameWithoutExtension(Url);
            var hash = _manifest.GetAssetBundleHash(bundleName);
            _request = hash.isValid ? UnityWebRequestAssetBundle.GetAssetBundle(Url, hash, 0)
                : UnityWebRequestAssetBundle.GetAssetBundle(Url);
            _request.disposeDownloadHandlerOnDispose = true;

            return _request.SendAsObserable()
                .ContinueWith(_ => 
                {
                    IsDownload = true;
                    return Observable.ReturnUnit();
                }).AsUnitObservable();
        }

        public void Clear()
        {
            _request?.Dispose();
            _request = null;
        }

        public override void Save() { }
    }
}