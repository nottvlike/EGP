namespace Asset
{
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;
    using UnityEngine;
    using ECS;
    using ECS.Common;
    using ECS.Module;
    using ECS.Data;

    internal class AssetCache
    {
        public string AssetName { get; private set; }
        public int Reference { get; private set; }

        public GameObject Asset => _asset;

        GameObject _asset;

        List<GameObject> _assetList = new List<GameObject>();

        static StringBuilder strBuilder = new StringBuilder();

        public AssetCache(string assetName, LoadedAssetInfo assetInfo)
        {
            AssetName = assetName;
            Reference = 0;
            
            _asset = assetInfo.asset as GameObject;

            if (assetInfo.instance != null)
            {
                _assetList.Add(assetInfo.instance as GameObject);
                Reference++;
            }
        }

        public void Prespawn(int count)
        {
            var unusedCount = _assetList.Where(_ => _.name.Contains(AssetConstant.UNUSED_ASSET_FLAG)).Count();
            if (unusedCount < count)
            {
                for (var i = unusedCount; i < count; i++)
                {
                    var obj = GameObject.Instantiate(_asset);
                    obj.SetActive(false);

                    strBuilder.Clear();
                    obj.name = strBuilder.Append(obj.name).Append(AssetConstant.UNUSED_ASSET_FLAG).ToString();
                    _assetList.Add(obj);
                }
            }
        }

        public GameObject Spawn()
        {
            var result = _assetList.Where(_ => _.name.Contains(AssetConstant.UNUSED_ASSET_FLAG))
                .FirstOrDefault();
            if (result == null)
            {
                result = GameObject.Instantiate(_asset);
                _assetList.Add(result);
            }

            strBuilder.Clear();
            result.name = strBuilder.Append(result.name).Replace(AssetConstant.UNUSED_ASSET_FLAG, string.Empty).ToString();
            result.SetActive(true);

            Reference++;
            return result;
        }

        public void Despawn(GameObject obj)
        {
#if DEBUG
            if (_assetList.IndexOf(obj) == -1)
            {
                Log.E("Destroy obj {0} failed, not {1}!", obj.ToString(), AssetName);
                return;
            }
#endif
            Reference--;

            obj.SetActive(false);
            strBuilder.Clear();
            obj.name = strBuilder.Append(obj.name).Append(AssetConstant.UNUSED_ASSET_FLAG).ToString();
        }

        public void Clear()
        {
            foreach (var asset in _assetList)
            {
                if (!asset.name.Contains(AssetConstant.UNUSED_ASSET_FLAG))
                {
                    asset.Despawn();
                }
                GameObject.Destroy(asset);
            }

            _assetList.Clear();
        }

        public void ClearUnusedAsset()
        {
            var clearAssetList = _assetList.Where(_ => _.name.Contains(AssetConstant.UNUSED_ASSET_FLAG)).ToArray();
            foreach (var asset in clearAssetList)
            {
                _assetList.Remove(asset);
                GameObject.Destroy(asset);
            }
        }
    }
}