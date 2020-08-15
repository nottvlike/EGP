namespace ECS.Module
{
    using GUnit = ECS.Unit.Unit;
    using ECS.Common;
    using ECS.Data;
    using ECS.Helper;
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using UniRx;
    using Asset;
    using UObject = UnityEngine.Object;
    using UnityEngine;

    public class AssetProcess : SingleModule
    {
        static Dictionary<UObject, string> _spawnedAssetNameDict = new Dictionary<UObject, string>();
        static Dictionary<UObject, string> _assetNameDict = new Dictionary<UObject, string>();
        static Dictionary<string, LoadedAssetInfo> _loadedAssetDict = new Dictionary<string, LoadedAssetInfo>();
        static Dictionary<string, AssetCache> _assetCacheDict = new Dictionary<string, AssetCache>();
 
        public override int Group { get; protected set; } 
            = WorldManager.Instance.Module.TagToModuleGroupType(AssetConstant.ASSET_MODULE_GROUP_NAME);

        public AssetProcess()
        {
            RequiredDataList = new Type[]{
                typeof(AssetProcessData)
            };
        }

        static AssetProcessData _processData;

        protected override void OnAdd(GUnit unit)
        {
            _processData = unit.GetData<AssetProcessData>();
        }

        protected override void OnRemove(GUnit unit)
        {
            _processData = null;
        }

        static IAssetLoader _assetLoader;
        public static IObservable<Unit> Init(IAssetLoader customLoader)
        {
            if (_assetLoader != null)
            {
                Log.W("AssetLoader has been inited!");
                return Observable.ReturnUnit();
            }

            _assetLoader = customLoader;
            if (_assetLoader == null)
            {
                _assetLoader = new AssetLoader();
            }

            return _assetLoader.Init().Do(_ =>
            {
                _processData.Manifest = _assetLoader.Manifest;
            });
        }

        public static T Get<T>(string assetPath) where T : UObject
        {
            if (_loadedAssetDict.ContainsKey(assetPath))
            {
                return _loadedAssetDict[assetPath].asset as T;
            }

            if (_assetCacheDict.ContainsKey(assetPath))
            {
                return _assetCacheDict[assetPath].Asset as T;
            }

            Log.E("Failed to get asset {0}, not loaded!", assetPath);
            return null;
        }

        static Dictionary<string, LoadingCache<UObject>> _loadingCacheDict = new Dictionary<string, LoadingCache<UObject>>();

        public static IObservable<T> Load<T>(string assetPath) where T : UObject
        {
            if (_loadedAssetDict.ContainsKey(assetPath))
            {
                return Observable.Return<T>(_loadedAssetDict[assetPath].asset as T);
            }

            if (_assetCacheDict.ContainsKey(assetPath))
            {
                return Observable.Return<T>(_assetCacheDict[assetPath].Asset as T);
            }

            LoadingCache<UObject> loadingCache = null;
            if (!_loadingCacheDict.TryGetValue(assetPath, out loadingCache))
            {
                loadingCache = new LoadingCache<UObject>();
                _loadingCacheDict.Add(assetPath, loadingCache);

                _assetLoader.Load<T>(assetPath)
                    .Do(_ =>
                    {
                        _loadedAssetDict.Add(assetPath, new LoadedAssetInfo() { asset = _, instance = null });
                        _assetNameDict.Add(_, assetPath);
                        _loadingCacheDict.Remove(assetPath);
                    }).Subscribe(loadingCache);
            }
            return loadingCache.ToLoadingObserable().Select(obj => obj as T);
        }

        internal static void Prespawn(UObject asset, int count = 1)
        {
#if DEBUG
            if (!_assetNameDict.ContainsKey(asset))
            {
                Log.W("Spawn asset {0} failed, should use the loaded asset!", asset);
                return;
            }
#endif
            var assetPath = _assetNameDict[asset];
            if (count == 1)
            {
                if (_loadedAssetDict.ContainsKey(assetPath))
                {
                    var loadedAssetInfo = _loadedAssetDict[assetPath];
                    if (loadedAssetInfo.instance == null)
                    {
                        var obj = UObject.Instantiate(asset);
                        loadedAssetInfo.instance = obj;
                        obj.name = obj.name + AssetConstant.UNUSED_ASSET_FLAG;
                        (obj as GameObject).SetActive(false);

                        _loadedAssetDict[assetPath] = loadedAssetInfo;
                    }
                    else if (!loadedAssetInfo.instance.name.Contains(AssetConstant.UNUSED_ASSET_FLAG))
                    {
                        var cache = new AssetCache(assetPath, loadedAssetInfo);
                        cache.Prespawn(count);

                        _assetCacheDict.Add(assetPath, cache);
                        _loadedAssetDict.Remove(assetPath);
                    }
                }
            }
            else
            {
                AssetCache cache;
                if (_loadedAssetDict.ContainsKey(assetPath))
                {
                    cache = new AssetCache(assetPath, _loadedAssetDict[assetPath]);
                    _assetCacheDict.Add(assetPath, cache);
                    _loadedAssetDict.Remove(assetPath);
                }
                else
                {
                    cache = _assetCacheDict[assetPath];
                }

                cache.Prespawn(count);
            }
        }

        internal static T Spawn<T>(UObject asset, int count = 1) where T : UObject
        {
#if DEBUG
            if (_spawnedAssetNameDict.ContainsKey(asset))
            {
                Log.W("Spawn {0} failed, perhaps has been spawned!", asset);
                return null;
            }

            if (!_assetNameDict.ContainsKey(asset))
            {
                Log.W("Spawn asset {0} failed, should use the loaded asset!", asset);
                return null;
            }
#endif

            var assetPath = _assetNameDict[asset];
            if (_assetCacheDict.ContainsKey(assetPath))
            {
                var cache = _assetCacheDict[assetPath];
                var obj = cache.Spawn();
                _spawnedAssetNameDict.Add(obj, assetPath);
                return obj as T;
            }

            var needNewInstance = false;
            if (_loadedAssetDict.ContainsKey(assetPath))
            {
                var loadAssetInfo = _loadedAssetDict[assetPath];
                if (loadAssetInfo.instance == null
                    || loadAssetInfo.instance.name.Contains(AssetConstant.UNUSED_ASSET_FLAG))
                {
                    var obj = loadAssetInfo.instance;
                    if (obj == null)
                    {
                        obj = UObject.Instantiate(asset);
                        loadAssetInfo.instance = obj;
                        _loadedAssetDict[assetPath] = loadAssetInfo;
                    }
                    else
                    {
                        (obj as GameObject).SetActive(true);
                    }

                    _spawnedAssetNameDict.Add(obj, assetPath);
                    obj.name = obj.name.Replace(AssetConstant.UNUSED_ASSET_FLAG, string.Empty);
                    return obj as T;
                }
                else
                {
                    needNewInstance = true;
                }
            }

            if (needNewInstance)
            {
                var loadAssetInfo = _loadedAssetDict[assetPath];
                var cache = new AssetCache(assetPath, loadAssetInfo);
                _assetCacheDict.Add(assetPath, cache);
                _loadedAssetDict.Remove(assetPath);

                var obj = cache.Spawn();
                _spawnedAssetNameDict.Add(obj, assetPath);
                return obj as T;
            }

            Log.W("Spawn obj {0} failed, unknown error!", asset);
            return null;
        }

        internal static void Despawn(UObject asset)
        {
#if DEBUG
            if (!_spawnedAssetNameDict.ContainsKey(asset))
            {
                Log.W("Despawn {0} failed, perhaps has been despawned!", asset);
                return;
            }
#endif

            var assetPath = _spawnedAssetNameDict[asset];
            if (_assetCacheDict.ContainsKey(assetPath))
            {
                var cache = _assetCacheDict[assetPath];
                cache.Despawn(asset as GameObject);
                _spawnedAssetNameDict.Remove(asset);
                return;
            }

            if (_loadedAssetDict.ContainsKey(assetPath))
            {
                var loadAssetInfo = _loadedAssetDict[assetPath];
                if (loadAssetInfo.instance == asset)
                {
                    asset.name = asset.name + AssetConstant.UNUSED_ASSET_FLAG;
                    (asset as GameObject).SetActive(false);
                    _spawnedAssetNameDict.Remove(asset);
                    return;
                }
            }

            Log.W("Despawn obj {0} failed, unknown error!", asset);
        }

        public static void ClearByAssetPrefix(string prefix)
        {
            var clearAssetList = new List<string>();

            var removedCacheList = _assetCacheDict.Where(_ => _.Key.Contains(prefix)).ToArray();
            foreach (var cache in removedCacheList)
            {
                cache.Value.Clear();
                _assetCacheDict.Remove(cache.Key);

                if (clearAssetList.IndexOf(cache.Key) == -1)
                {
                    clearAssetList.Add(cache.Key);
                }
            }

            var removedAssetList = _loadedAssetDict.Where(_ => _.Key.Contains(prefix)).ToArray();
            foreach (var asset in removedAssetList)
            {
                var instance = asset.Value.instance;
                if (instance != null)
                {
                    if (!instance.name.Contains(AssetConstant.UNUSED_ASSET_FLAG))
                    {
                        (instance as GameObject)?.Despawn();
                    }
                    UObject.Destroy(asset.Value.instance);
                }

                _loadedAssetDict.Remove(asset.Key);

                if (clearAssetList.IndexOf(asset.Key) == -1)
                {
                    clearAssetList.Add(asset.Key);
                }
            }

            var removedAssetNameList = _assetNameDict.Where(_ => clearAssetList.IndexOf(_.Value) != -1)
                .Select(_ => _.Key).ToArray();
            foreach (var asset in removedAssetNameList)
            {
                _assetNameDict.Remove(asset);
            }

            removedAssetNameList = _spawnedAssetNameDict.Where(_ => clearAssetList.IndexOf(_.Value) != -1)
                .Select(_ => _.Key).ToArray();
            foreach (var asset in removedAssetNameList)
            {
                GameObject.Destroy(asset);
                _spawnedAssetNameDict.Remove(asset);
            }

            foreach (var asset in clearAssetList)
            {
                _assetLoader.Clear(asset);
            }
        }

        public static void ClearUnusedAsset()
        {
            foreach (var assetCache in _assetCacheDict.Values)
            {
                assetCache.ClearUnusedAsset();
            }

            var clearAssetList = new List<string>();
            var removedCacheList = _assetCacheDict.Where(_ => _.Value.Reference == 0).ToArray();
            foreach (var cache in removedCacheList)
            {
                cache.Value.Clear();
                _assetCacheDict.Remove(cache.Key);

                if (clearAssetList.IndexOf(cache.Key) == -1)
                {
                    clearAssetList.Add(cache.Key);
                }
            }

            var removedAssetList = _loadedAssetDict.Where(_ => _.Value.instance == null
                || _.Value.instance.name.Contains(AssetConstant.UNUSED_ASSET_FLAG)).ToArray();
            foreach (var asset in removedAssetList)
            {
                if (asset.Value.instance != null)
                {
                    UObject.Destroy(asset.Value.instance);
                }

                _loadedAssetDict.Remove(asset.Key);

                if (clearAssetList.IndexOf(asset.Key) == -1)
                {
                    clearAssetList.Add(asset.Key);
                }
            }

            var removedAssetNameList = _assetNameDict.Where(_ => clearAssetList.IndexOf(_.Value) != -1)
                .Select(_ => _.Key).ToArray();
            foreach (var asset in removedAssetNameList)
            {
                _assetNameDict.Remove(asset);
            }

            removedAssetNameList = _spawnedAssetNameDict.Where(_ => clearAssetList.IndexOf(_.Value) != -1)
                .Select(_ => _.Key).ToArray();
            foreach (var asset in removedAssetNameList)
            {
                GameObject.Destroy(asset);
                _spawnedAssetNameDict.Remove(asset);
            }

            foreach (var asset in clearAssetList)
            {
                _assetLoader.Clear(asset);
            }
        }
    }
}