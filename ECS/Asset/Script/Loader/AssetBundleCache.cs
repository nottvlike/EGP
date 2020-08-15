namespace Asset
{
    using UObject = UnityEngine.Object;
    using System;
    using UniRx;
    using UnityEngine;
    using System.Linq;
    using ECS.Common;

    internal class AssetBundleCache
    {
        string _bundleName;
        public int BundleReference { get; private set; }

        AssetBundle _assetBundle;

        public AssetBundleCache(string bundleName, AssetBundle assetBundle)
        {
            _bundleName = bundleName;
            _assetBundle = assetBundle;
            BundleReference = 0;
        }

        public IDisposable Reference()
        {
            BundleReference++;
            return Disposable.Create(() => Unreference());
        }

        public void Unreference()
        {
            BundleReference--;
            if (BundleReference < 0)
            {
                Log.W("Asset {0} reference less than zero!", _bundleName);
                BundleReference = 0;
            }
        }

        public IObservable<UObject> Load(string assetName, bool isLoadAll = false)
        {
            if (isLoadAll)
            {
                return _assetBundle.LoadAllAsObserable().SelectMany(_ => _.allAssets);
            }
            else
            {
                return _assetBundle.LoadAsObserable<UObject>(assetName).Select(_ => _.asset);
            }
        }

        public void Unload(bool unloadAll = false)
        {
            _assetBundle.Unload(unloadAll);
        }
    }
}